// Licensed under the Apache 2.0 License. See LICENSE.txt in the project root for more information.

using System;
using System.Collections.Generic;

namespace ElasticLinq.Test.TestSupport
{
    public class Robot
    {
        public Int32 Id { get; set; }
        public string Name { get; set; }
        public DateTime Started { get; set; }
        public decimal Cost { get; set; }
        public double EnergyUse { get; set; }
        public int? Zone { get; set; }
        public List<string> Aliases { get; set; }
        public RobotStats Stats { get; set; }

        public Robot()
        {
            Stats = new RobotStats();
        }
    }

    public class RobotStats
    {
        public RobotLimbs Limbs { get; set; }
        public RobotPricing Pricing { get; set; }

        public RobotStats()
        {
            Limbs = new RobotLimbs();
            Pricing = new RobotPricing();
        }
    }

    public class RobotLimbs
    {
        public Int32 HandCount { get; set; }
    }

    public class RobotPricing
    {
        public decimal InvoicePrice { get; set; }
    }

    public static class RobotFactory
    {
        public static List<Robot> Inventory = new List<Robot>
        {
            new Robot { Id = 1, Name = "Kryten", Zone = 1, Started = new DateTime(2030, 5, 3), Cost = 1.2m, EnergyUse = 5.1 },
            new Robot { Id = 2, Name = "Robbie", Zone = 2, Started = new DateTime(1950, 1, 1), Cost = 6.2m, EnergyUse = 2.102 },
            new Robot { Id = 3, Name = "IG-88", Zone = 3, Started = new DateTime(1290, 1, 1), Cost = 15.2m, EnergyUse = 51.5 },
            new Robot { Id = 4, Name = "R2D2", Zone = 3, Started = new DateTime(1295, 8, 5), Cost = 9.2m, EnergyUse = 9.4 },
            new Robot { Id = 5, Name = "C3PO", Zone = 3, Started = new DateTime(1298, 3, 1), Cost = 3.2m, EnergyUse = 78.0 }
        };
    }
}