using System.Text.RegularExpressions;
using Domain.Products.Exceptions;

namespace Domain.Products.Entities;

public class ImageProduct : Domain.Base.Entities.BaseEntity
{
    public int ProductId { get; protected set; }

    public int Position { get; protected set; }

    public string Url { get; protected set; }

    public Product Product { get; protected set; }

    private readonly Regex _imageRegex = new Regex(@"(\W)(jpg|jpeg|png|gif|webp)", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    public ImageProduct(int productId, int position, string url, int id = 0)
    {
        if (id != 0)
            this.Id = id;

        this.ProductId = productId;
        this.Position = position;
        this.Url = url;

        CheckImageUrlFormat();
        CheckIfIsAValidPosition();
    }

    protected ImageProduct()
    {
    }

    public void UpdateImageProduct(ImageProduct imageToUpdate)
    {
        this.Position = imageToUpdate.Position;
        this.Url = imageToUpdate.Url;
        this.UpdatedAt = DateTimeOffset.Now;
    }
    
    private void CheckImageUrlFormat()
    {
        if (!Uri.IsWellFormedUriString(this.Url, UriKind.Absolute))
            throw new UrlNotValidException();

        if (!_imageRegex.IsMatch(this.Url))
            throw new UrlIsNotAnImageException();
    }

    private void CheckIfIsAValidPosition()
    {
        if (this.Position <= 0)
            throw new ImagePositionException();
    }
}