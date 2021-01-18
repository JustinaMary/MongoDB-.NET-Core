using GraphQL.Language.AST;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using PetizenApi.Database;
using PetizenApi.Interfaces;
using PetizenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PetizenApi.Repositories
{
    public class TicketingRepository : ITicketingRepository
    {
        private readonly MongoConnection context = null;
        private readonly ICommonRepository _commonRepository;

        public TicketingRepository(IOptions<MongoSettings> settings, ICommonRepository commonRepository)
        {
            context = new MongoConnection(settings);
            _commonRepository = commonRepository;
        }

        #region Ticket Booking

        public async Task<List<Ticketing>> GetTicketingAsync(string tId, string locationId, int day, IDictionary<string, Field> fields, CancellationToken cancellationToken)
        {
            try
            {
                var builder = Builders<Ticketing>.Filter;
                var filterDefine = FilterDefinition<Ticketing>.Empty;

                if (!string.IsNullOrEmpty(tId))
                {
                    filterDefine = builder.Eq(d => d.TId, tId);
                }
                if (!string.IsNullOrEmpty(locationId))
                {
                    filterDefine = filterDefine & builder.Eq(d => d.LocationId, locationId);
                }
                if (day != 0)
                {
                    filterDefine = filterDefine & builder.Eq(d => d.Day, day);
                }

                var Result = new List<Ticketing>();
                if (fields == null)
                {
                    Result = await context.Ticketing
                                 .Find(filterDefine)
                                 .ToListAsync().ConfigureAwait(false);
                }
                else
                {
                    var fieldsName = _commonRepository.GetFieldsName(fields);
                    var fieldsBuilder = Builders<Ticketing>.Projection;
                    var fieldsAdd = fieldsBuilder.Include(fieldsName[0]);
                    foreach (var item in fieldsName.Skip(1))
                    {
                        fieldsAdd = fieldsAdd.Include(item);
                    }

                    Result = await context.Ticketing
                                 .Find(filterDefine)
                                  .Project<Ticketing>(fieldsAdd)
                                 .ToListAsync().ConfigureAwait(false);
                }
                return Result;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<Ticketing> InsUpdTicketingAsync(Ticketing ticketing, CancellationToken cancellationToken)
        {
            try
            {

                if (ticketing == null) throw new ArgumentNullException(nameof(ticketing));

                if (string.IsNullOrEmpty(ticketing.TId))
                {
                    await context.Ticketing.InsertOneAsync(ticketing).ConfigureAwait(false);
                }
                else
                {
                    //var tktBooking = await GetTicketing(ticketing.TId, "",0, null);
                    var queryBuilder = Builders<Ticketing>.Filter;
                    var filterDefine = FilterDefinition<Ticketing>.Empty;

                    var update = Builders<Ticketing>.Update
                                       .Set(x => x.StartTime, ticketing.StartTime)
                                       .Set(x => x.EndTime, ticketing.EndTime)
                                       .Set(x => x.BreakStartTime, ticketing.BreakStartTime)
                                       .Set(x => x.BreakEndTime, ticketing.BreakEndTime)
                                       .Set(x => x.IsHoliday, ticketing.IsHoliday);


                    await context.Ticketing.UpdateOneAsync(t => t.TId == ticketing.TId, update).ConfigureAwait(false);


                }
                return ticketing;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<bool> DeleteTicketingAsync(string TId, CancellationToken cancellationToken)
        {
            try
            {
                await context.Ticketing.DeleteOneAsync(
                        Builders<Ticketing>.Filter.Eq("TId", TId)).ConfigureAwait(false);

                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        #endregion


    }
}


