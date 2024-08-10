
// NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class Library
{

    private LibraryBook[] booksField;

    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("Book", IsNullable = false)]
    public LibraryBook[] Books
    {
        get
        {
            return this.booksField;
        }
        set
        {
            this.booksField = value;
        }
    }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class LibraryBook
{

    private string titleField;

    private string authorField;

    private ushort yearField;

    private string genreField;

    /// <remarks/>
    public string Title
    {
        get
        {
            return this.titleField;
        }
        set
        {
            this.titleField = value;
        }
    }

    /// <remarks/>
    public string Author
    {
        get
        {
            return this.authorField;
        }
        set
        {
            this.authorField = value;
        }
    }

    /// <remarks/>
    public ushort Year
    {
        get
        {
            return this.yearField;
        }
        set
        {
            this.yearField = value;
        }
    }

    /// <remarks/>
    public string Genre
    {
        get
        {
            return this.genreField;
        }
        set
        {
            this.genreField = value;
        }
    }
}

