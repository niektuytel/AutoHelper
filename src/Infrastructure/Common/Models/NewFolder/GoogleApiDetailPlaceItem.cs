using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoHelper.Infrastructure.Common.Models.NewFolder;
internal class GoogleApiDetailPlaceItem
{
    public object[] html_attributions { get; set; }
    public Result result { get; set; }
    public string status { get; set; }
}

public class Result
{
    public string place_id { get; set; }
    public string business_status { get; set; }
    public string formatted_phone_number { get; set; }
    public Geometry geometry { get; set; }
    public string icon { get; set; }
    public string icon_background_color { get; set; }
    public string icon_mask_base_uri { get; set; }
    public string name { get; set; }
    public Opening_Hours opening_hours { get; set; }
    public Photo[] photos { get; set; }
    public Review[] reviews { get; set; }
    public float rating { get; set; }
    public int user_ratings_total { get; set; }
    public string website { get; set; }
}

public class Geometry
{
    public Location location { get; set; }
    public Viewport viewport { get; set; }
}

public class Location
{
    public float lat { get; set; }
    public float lng { get; set; }
}

public class Viewport
{
    public Northeast northeast { get; set; }
    public Southwest southwest { get; set; }
}

public class Northeast
{
    public float lat { get; set; }
    public float lng { get; set; }
}

public class Southwest
{
    public float lat { get; set; }
    public float lng { get; set; }
}

public class Opening_Hours
{
    public bool open_now { get; set; }
    public Period[] periods { get; set; }
    public string[] weekday_text { get; set; }
}

public class Period
{
    public Close close { get; set; }
    public Open open { get; set; }
}

public class Close
{
    public int day { get; set; }
    public string time { get; set; }
}

public class Open
{
    public int day { get; set; }
    public string time { get; set; }
}

public class Photo
{
    public int height { get; set; }
    public string[] html_attributions { get; set; }
    public string photo_reference { get; set; }
    public int width { get; set; }
}

public class Review
{
    public string author_name { get; set; }
    public string author_url { get; set; }
    public string language { get; set; }
    public string original_language { get; set; }
    public string profile_photo_url { get; set; }
    public int rating { get; set; }
    public string relative_time_description { get; set; }
    public string text { get; set; }
    public int time { get; set; }
    public bool translated { get; set; }
}
