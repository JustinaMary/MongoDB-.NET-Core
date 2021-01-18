using System;
using System.Reflection;

namespace PetizenApi.Providers
{


    public class MethodNames : IMethodNames
    {
        public static readonly string[] DogBreeds = new string[] { "PetizenApi.Models", "DogRepository", "GetDogBreedAsync", "InsUpdDogBreedAsync", "Single" };

        public static readonly string[] DogMedia = new string[] { "PetizenApi.Models", "DogRepository", "GetDogsMediaAsync", "InsUpdDogMediaAsync", "Single" };

        public static readonly string[] UserLocationMedia = new string[] { "PetizenApi.Models", "LocationServiceRepository", "GetUserLocationMediaAsync", "InsUpdUserLocationMediaAsync", "Single" };

        public static readonly string[] DogCourses = new string[] { "PetizenApi.Models", "DogRepository", "GetDogCoursesAsync", "InsUpdDogCoursesAsync", "Single" };


        public object Getmethodname(string TableName)
        {
            try
            {
                FieldInfo fld = typeof(MethodNames).GetField(TableName);

                object ouput = fld.GetValue(null);

                return ouput;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }
    }

    public interface IMethodNames
    {
        object Getmethodname(string TableName);
    }
}
