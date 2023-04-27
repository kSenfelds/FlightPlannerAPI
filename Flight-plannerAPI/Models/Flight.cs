﻿namespace Flight_plannerAPI.Models
{
    public class Flight
    {
        public int Id { get; set; }
        public Airport From { get; set; }
        public Airport To { get; set; }
        public string Carrier { get; set; }
        public string DepartureTime { get; set; }
        public string ArrivalTime { get; set; }

    }

    /*
     *  id: number;
  from: Airport;
  to: Airport;
  carrier: string;
  departureTime: Date;
  arrivalTime: Date;
     */
}