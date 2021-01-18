using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using PetizenApi.Providers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;


namespace PetizenApi.Repositories
{
    public class UploadRepository
    {
        private readonly IOptions<MongoSettings> _context = null;
        private readonly ConcurrentBag<FileClass> _files;
        private readonly string _uploadDirectory;
        private readonly IMethodNames _methodname;
        private readonly IAccountRepository _accountRepository;
        string MediaUrl = string.Empty;


        public UploadRepository(IOptions<MongoSettings> settings, IAccountRepository accountRepository, IOptions<ApplicationUrl> webUrl)
        {
            _files = new ConcurrentBag<FileClass>();
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot//Media");
            Directory.CreateDirectory(_uploadDirectory);
            _methodname = new MethodNames();
            _context = settings;
            _accountRepository = accountRepository;
            if (webUrl != null)
            {
                if (webUrl == null) throw new ArgumentNullException(nameof(webUrl));
                MediaUrl = webUrl.Value.MediaUrl.ToString();
            }

        }

        public IEnumerable<FileClass> Files => _files;

        public async Task<FileClass> SaveAsync(IFormFile formFile, string FileName)
        {

            var path = Path.Combine(_uploadDirectory, FileName);

            if (formFile == null) throw new ArgumentNullException(nameof(formFile));

            using (var fs = formFile.OpenReadStream())
            using (var ws = System.IO.File.Create(path))
            {
                await fs.CopyToAsync(ws).ConfigureAwait(false);
            }

            var file = new FileClass
            {
                Id = FileName,
                MimeType = formFile.ContentType,
                Name = formFile.FileName,
                Path = Path.Combine(MediaUrl, FileName)
            };
            _files.Add(file);

            return file;
        }



        public async Task<FileClass> UploadFilesAsync(IFormFile File)
        {
            try
            {
                string TableName = "";
                string ColumnName = "";
                // Basic validation on mime types and file extension
                string[] FilesExt = { ".pdf", ".png", ".jpg", ".mp4", ".webm", ".ogg", ".jpeg" };
                // Get File Extension

                if (File == null) throw new ArgumentNullException(nameof(File));

                string Extension = Path.GetExtension(File.FileName);
                string FileName = "";
                var file = new FileClass();

                if (Array.IndexOf(FilesExt, Extension) >= 0)
                {
                    string primaryid = "";
                    string FilePath = "";
                    //string ImageType = "";
                    if (File != null)
                    {

                        var Filesplit = File.FileName.Split('.');
                        var filesplitName = Filesplit[0].Split("_");
                        primaryid = filesplitName[0];
                        TableName = filesplitName[1];
                        ColumnName = filesplitName[2];
                        // ImageType = Convert.ToString(filesplitName[2]);
                        var extension = "." + File.FileName.Split('.')[File.FileName.Split('.').Length - 1];
                        FileName = Guid.NewGuid().ToString() + extension;

                        //file = await Save(File, FileName);

                        file = await SaveAsync(File, FileName).ConfigureAwait(false);

                        //var UserDetails = await _accountRepository.GetUserMasterAsync(50, "", "", 0, cancellationToken).ConfigureAwait(false);



                        FilePath = FileName;
                    }
                    //to get the methods of the Entity(Model)

                    object table = _methodname.Getmethodname(TableName);
                    string[] tablearray = ((IEnumerable)table).Cast<object>()
                                                     .Select(x => x.ToString())
                                                     .ToArray();


                    var entity = GetUpdateMethod(tablearray[1], tablearray[2], primaryid);

                    dynamic dynamicentity = entity;
                    var singleEntity = dynamicentity.Result[0];
                    //sets the value to the property
                    PropertyInfo propertyInfo = singleEntity.GetType().GetProperty(ColumnName);
                    propertyInfo.SetValue(singleEntity, Convert.ChangeType(FilePath, propertyInfo.PropertyType, CultureInfo.CurrentCulture), null);

                    string classname = tablearray[0] + "." + TableName; //to get the complete class name with name space
                    Type type = Type.GetType(classname);
                    var listType = typeof(List<>);
                    var constructedListType = listType.MakeGenericType(type);

                    if (tablearray[4] == "Multiple")
                    {
                        var classinstance = (IList)Activator.CreateInstance(constructedListType);

                        classinstance.Add(singleEntity);
                        PostUpdateMethod(tablearray[1], tablearray[3], classinstance);
                    }
                    else if (tablearray[4] == "Single")
                    {
                        var classinstance = Activator.CreateInstance(constructedListType);
                        classinstance = singleEntity;
                        PostUpdateMethod(tablearray[1], tablearray[3], classinstance);
                    }




                    return file;
                }
                else
                {
                    return file;
                }


            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

        }

        public object GetUpdateMethod(string typename, string methodName, string Primaryid)
        {
            try
            {
                Type type = Assembly.GetExecutingAssembly().GetType("PetizenApi.Repositories." + typename); //all method will have same namespace
                var paramcount = type.GetConstructors()[0].GetParameters().Length;

                var parameter = new List<object>();
                parameter.Add(_context);
                for (int i = 1; i < paramcount; ++i)
                {
                    parameter.Add(null);
                }
                var parameterarray = parameter.ToArray();

                object instance = Activator.CreateInstance(type, parameterarray);

                MethodInfo theMethod = type.GetMethod(methodName); //to invoke the method dynamically

                ParameterInfo[] pars = theMethod.GetParameters();
                int count = pars.Length;// to get the parameter count
                List<object> parameters = new List<object>();
                parameters.Add(Primaryid);
                for (int i = 1; i < count; i++)
                {
                    parameters.Add(null);
                }
                object[] newparameter = parameters.ToArray();
                var output = theMethod.Invoke(instance, newparameter);
                return output;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }

        public void PostUpdateMethod(string typename, string methodName, object entity)
        {
            try
            {
                Type type = Assembly.GetExecutingAssembly().GetType("PetizenApi.Repositories." + typename); //all method will have same namespace

                var paramcount = type.GetConstructors()[0].GetParameters().Length;

                var parameter = new List<object>();
                parameter.Add(_context);
                for (int i = 1; i < paramcount; ++i)
                {
                    parameter.Add(null);
                }
                var parameterarray = parameter.ToArray();

                object instance = Activator.CreateInstance(type, parameterarray);
                MethodInfo theMethod = type.GetMethod(methodName); //to invoke the method dynamically

                ParameterInfo[] pars = theMethod.GetParameters();
                int count = pars.Length;// to get the parameter count
                List<object> parameters = new List<object>();

                parameters.Add(entity);
                for (int i = 1; i < count; i++)
                {
                    parameters.Add(null);
                }
                object[] newparameter = parameters.ToArray();
                var output = theMethod.Invoke(instance, newparameter);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }


        }

    }



}
