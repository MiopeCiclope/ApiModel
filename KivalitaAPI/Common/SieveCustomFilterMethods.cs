using KivalitaAPI.Models;
using Sieve.Services;
using System;
using System.Linq;

public class SieveCustomFilterMethods : ISieveCustomFilterMethods
{
    public IQueryable<Leads> Tags(IQueryable<Leads> source, string op, string[] value) // The method is given the {Operator} & {Value}
    {
        var idValues = value.Select(v => int.Parse(v));
        var result = source.Where(lead => lead.LeadTag.Where(l => idValues.Any(id => id == l.TagId)).Any());

        return result;
    }

    public IQueryable<Post> LastChangeDate(IQueryable<Post> source, string op, string[] value)
    {
        var lastDate = DateTime.Parse(value[0]);
        var result = source.Where(entity => entity.CreatedAt > lastDate || entity.UpdatedAt > lastDate);

        return result;
    }

    public IQueryable<Job> LastChangeDate(IQueryable<Job> source, string op, string[] value)
    {
        var lastDate = DateTime.Parse(value[0]);
        var result = source.Where(entity => entity.CreatedAt > lastDate || entity.UpdatedAt > lastDate);

        return result;
    }

    public IQueryable<FlowTask> ShowAutomatic(IQueryable<FlowTask> source, string op, string[] value)
    {
        var showAutomatic = value[0] == "true";
        var result = (showAutomatic) 
                ? source.Where(entity => entity.FlowAction.Flow.IsActive)
                : source.Where(entity =>
                            entity.FlowAction.Flow.IsActive
                            && (
                                (entity.FlowAction.Type == "email" && !entity.FlowAction.Flow.isAutomatic) 
                                || entity.FlowAction.Type != "email"
                            )
                        );

        return result;
    }

}