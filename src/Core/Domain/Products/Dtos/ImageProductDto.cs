using System.Text.Json.Serialization;
using Domain.Products.Entities;

namespace Domain.Products.Dtos;

public class ImageProductDto
{
    public int? Id { get; private set; }

    public int Position { get; set; }

    public string Url { get; set; }

    [JsonConstructor]
    public ImageProductDto(int? id, int position, string url)
    {
        this.Id = id;
        this.Position = position;
        this.Url = url;
    }

    public ImageProductDto(ImageProduct imageProduct)
    {
        this.Id = imageProduct.Id;
        this.Position = imageProduct.Position;
        this.Url = imageProduct.Url;
    }
}