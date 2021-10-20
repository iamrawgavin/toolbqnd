using System;
using System.Collections.Generic;
using Xunit;
using SetlistLoader;


namespace ExtensionTests
{
    public class ListTests
    {
        [Fact]
        public void List_of_5_returns_3_lists_if_batch_2()
        {
            int expectedLists = 3;
            int expectedRecords = 5;
            int batchSize = 2;

            List<Concert> all = new List<Concert>();
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2001, 11, 1) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(1999, 10, 10) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2002, 8, 24) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2009, 7, 27) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2007, 12, 11) });

            List<List<Concert>> lol = all.GetListOfLists(batchSize);
            Assert.Equal(expectedLists, lol.Count);

            int records = 0;
            foreach (var list in lol)
            {
                records += list.Count;
            }
            Assert.Equal(expectedRecords, records);
        }

        [Fact]
        public void List_of_5_returns_2_lists_if_batch_3()
        {
            int expectedLists = 2;
            int expectedRecords = 5;
            int batchSize = 3;

            List<Concert> all = new List<Concert>();
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2001, 11, 1) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(1999, 10, 10) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2002, 8, 24) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2009, 7, 27) });
            all.Add(new Concert() { Artist = "tool", Date = new System.DateTime(2007, 12, 11) });

            List<List<Concert>> lol = all.GetListOfLists(batchSize);
            Assert.Equal(expectedLists, lol.Count);

            int records = 0;
            foreach (var list in lol)
            {
                records += list.Count;
            }
            Assert.Equal(expectedRecords, records);
        }
    }
}
