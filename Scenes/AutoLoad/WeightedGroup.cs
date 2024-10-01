using System;
using System.Collections.Generic;
using System.Linq;


// type T will be our class and the double will be the prob for the thing to spawn
// where T must be a class, for a safety measurement
public class WeightedGroup<T>: Dictionary<T, double> where T : class 
{
    public T GetItem()
    {
        double totalWeight = 0;
        foreach (var item in Values)
        {
            var absValue = Math.Abs(item);
            totalWeight += absValue;
        }
        
        if (totalWeight <= 0)
        {
            return null;
        }

        double roll = new Random().NextDouble();
        double total = 0;
        
        // taking the weight of all the objects, making them into a list to be able to sort them by descending
        foreach (KeyValuePair<T, double> kvp in this.ToList().OrderByDescending(v => v.Value))
        {
            // this is done to get the weight down to a decimal number. ex 50 / 100 = 0.5
            total += kvp.Value / totalWeight;
            if (roll <= total)
            {
                return kvp.Key;
            }
        }

        // this is just if something funky happened, then we just return the first object in our dict
        return Keys.FirstOrDefault();
    }
}