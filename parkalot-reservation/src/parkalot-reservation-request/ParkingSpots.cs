namespace parkalot_reservation_request;

public static class ParkingSpots
{
    public static List<string> GetSelectedSpots()
    {
        var parkingSpots = new Dictionary<int, string>()
        {
            // {1, "a"},
            // {2, "aa"},
            // {3, "aaa"},
            // {4, "aaaa"},
            // {5, "aaaaa"},
            // {6, "aaaaaa"},
            // {7, "aaaaaaa"},
            // {8, "aaaaaaaa"},
            // {9, "aaaaaaaaa"},
            // {10, "b"},
            // {11, "ba"},
            // {12, "baa"},
            // {13, "baaa"},
            // {14, "baaaa"},
            // {15, "baaaaa"},
            // {16, "baaaaaa"},
            // {17, "baaaaaaa"},
            // {18, "baaaaaaaa"},
            // {19, "baaaaaaaaa"},
            // {20, "bb"},
            // {21, "bba"},
            // {22, "bbaa"},
            // {23, "bbaaa"},
            // {24, "bbaaaa"},
            // {25, "bbaaaaa"},
            // {26, "bbaaaaaa"},
            // {27, "bbaaaaaaa"},
            // {28, "bbaaaaaaaa"},
            // {29, "bbaaaaaaaaa"},
            // {30, "bbb"},
            // {31, "bbba"},
            // {32, "bbbaa"},
            // {33, "bbbaaa"},
            // {34, "bbbaaaa"},
            // {35, "bbbaaaaa"},
            // {36, "bbbaaaaaa"},
            // {37, "bbbaaaaaaa"},
            // {38, "bbbaaaaaaaa"},
            {39, "bbbaaaaaaaaa"},
            {40, "bbbb"}
        };
        
        return parkingSpots.Values.ToList();
    }
}