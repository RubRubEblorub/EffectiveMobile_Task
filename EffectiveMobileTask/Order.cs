using System.Globalization;
using System.Text.Json.Serialization;

namespace EffectiveMobileTask;

public class Order
{
    private enum _cityDistricts
    {
        CenterD = 1,
        FarD,
        RightD,
        LeftD,
    }
    
    public int _id { get; set; }
    public int _weight { get; set; }
    public string _cityDistrict { get; set; }
    public DateTime _deliveryDateTime { get; set; }
    
    public Order() {}
    
    public Order(int id, int weight, int cityDistrict, string deliveryDateTime)
    {
        _id = id;
        _weight = weight;
        _cityDistrict = Enum.GetName(typeof(_cityDistricts), cityDistrict);
        _deliveryDateTime = DateTime.ParseExact(deliveryDateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
    }

    public int GetId()
    {
        return _id;
    }

    public string GetCityDistrict()
    {
        return _cityDistrict;
    }
    
    public override string ToString()
    {
        return $"Id: {_id} | Weight: {_weight} | City district: {_cityDistrict} | Delivery date: {_deliveryDateTime}";
    }
    
    public static Array GetCityDistricts()
    {
        return Enum.GetValues(typeof(_cityDistricts));
    }

    public static string GetCityDistrictNameFromEnum(int value)
    {
        return Enum.GetName(typeof(_cityDistricts), value);
    }
}