namespace hotstack.Config
{
    public interface IConfigureServerSettings
    {
        IConfigureServerSettings BasePath( string path );
        IConfigureServerSettings DefaultLayoutTemplate( string layoutName );
        IConfigureServerSettings AddViewSearchFolder( string path );
    }
}