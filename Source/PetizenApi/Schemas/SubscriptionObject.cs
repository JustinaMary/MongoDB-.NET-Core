namespace PetizenApi.Schemas
{
    using GraphQL.Types;

    /// <summary>
    /// All subscriptions defined in the schema used to be notified of changes in data.
    /// </summary>
    /// <example>
    /// <c>
    /// subscription whenHumanCreated {
    ///   humanCreated(homePlanets: ["Earth"])
    ///   {
    ///     id
    ///     name
    ///     dateOfBirth
    ///     homePlanet
    ///     appearsIn
    ///   }
    /// }
    /// </c>
    /// The is an example subscription to be notified when a human is created.
    /// </example>
    public class SubscriptionObject : ObjectGraphType<object>
    {
        //public SubscriptionObject(IHumanRepository humanRepository)
        //{
        //    this.Name = "Subscription";
        //    this.Description = "The subscription type, represents all updates can be pushed to the client in real time over web sockets.";

        //    this.AddField(
        //        new EventStreamFieldType()
        //        {
        //            Name = "humanCreated",
        //            Description = "Subscribe to human created events.",
        //            Arguments = new QueryArguments(
        //                new QueryArgument<ListGraphType<StringGraphType>>()
        //                {
        //                    Name = "homePlanets",
        //                }),
        //            Type = typeof(HumanCreatedEvent),
        //            Resolver = new FuncFieldResolver<Human>(context => context.Source as Human),
        //            Subscriber = new EventStreamResolver<Human>(context =>
        //            {
        //                var homePlanets = context.GetArgument<List<string>>("homePlanets");
        //                return humanRepository
        //                    .WhenHumanCreated
        //                    .Where(x => homePlanets is null || homePlanets.Contains(x.HomePlanet));
        //            }),
        //        });
        //}
    }
}
