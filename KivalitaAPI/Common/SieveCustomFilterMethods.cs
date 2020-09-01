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
}