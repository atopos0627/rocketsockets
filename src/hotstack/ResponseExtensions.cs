using System;
using System.IO;
using System.Text;
using hotstack.Owin.Impl;
using hotstack.View;
using Symbiote.Core;
using Symbiote.Core.Extensions;
using Symbiote.Core.Serialization;

namespace hotstack
{
    public static class ResponseExtensions
    {
        public static IBuildResponse AppendToBody( this IBuildResponse builder, byte[] bytes )
        {
            var helper = builder as ResponseHelper;
			helper.ResponseChunks.Add( bytes );
            return builder;
        }

        public static IBuildResponse AppendToBody( this IBuildResponse builder, string text )
        {
            var helper = builder as ResponseHelper;
            return AppendToBody( builder, helper.Encoder( text ) );
        }

        public static IBuildResponse AppendFileContentToBody( this IBuildResponse builder, string path )
        {
            var helper = builder as ResponseHelper;
            path = path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var fullPath = Path.Combine( helper.Configuration.BaseContentPath, path );
            if ( !File.Exists( fullPath ) )
            {
                throw new FileNotFoundException( "Requested file could not be found: '{0}'".AsFormat( fullPath ) );
            }

            using( var stream = new FileStream( fullPath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
            {
                var bytes = stream.Length;
                var buffer = new byte[bytes];
                stream.Read( buffer, 0, (int) bytes );
                helper.AppendToBody( buffer );
                var contentType = helper.GetContentTypeFromPath( fullPath );
                helper.HeaderDefinitions.ContentType( contentType );
            }

            return helper;
        }

        public static IBuildResponse AppendJson<T>( this IBuildResponse builder, T item )
        {
            var helper = builder as ResponseHelper;
            return helper.AppendToBody( item.ToJson() );
        }

        public static IBuildResponse AppendProtocolBuffer<T>( this IBuildResponse builder, T item )
        {
            var helper = builder as ResponseHelper;
            return helper.AppendToBody( item.ToProtocolBuffer() );
        }

        public static IBuildResponse DefineHeaders( this IBuildResponse builder, Action<IDefineHeaders> headerDefinition )
        {
            var helper = builder as ResponseHelper;
            headerDefinition( helper.HeaderDefinitions );
            return builder;
        }

        public static IBuildResponse EncodeStringsWith( this IBuildResponse builder, Func<string, byte[]> encoder )
        {
            var helper = builder as ResponseHelper;
            helper.Encoder = encoder;
            return builder;
        }

        public static IBuildResponse RenderView<TModel>( this IBuildResponse builder, TModel model, string viewName )
        {
            var helper = builder as ResponseHelper;
            if( !string.IsNullOrEmpty( helper.Configuration.DefaultLayoutTemplate ) )
                return builder.RenderView( model, viewName, helper.Configuration.DefaultLayoutTemplate );

            var engine = Assimilate.GetInstanceOf<IViewEngine>();
            using( var stream = new MemoryStream() )
            using( var streamWriter = new StreamWriter( stream, Encoding.UTF8 ) )
            {
                try
                {
                    engine.Render( viewName, model, streamWriter );
                }
                catch ( Exception e )
                {
                    streamWriter.Write( helper.RENDER_EXCEPTION_TEMPLATE.AsFormat( e ) );
                    streamWriter.Flush();
                }
                streamWriter.Flush();
                helper.DefineHeaders( x => x.ContentType( ContentType.Html ).ContentLength( stream.Length ) );
                helper.AppendToBody( stream.GetBuffer() );
            }
            return builder;
        }

        public static IBuildResponse RenderView<TModel>( this IBuildResponse builder, TModel model, string viewName, string layoutName )
        {
            var helper = builder as ResponseHelper;
            var engine = Assimilate.GetInstanceOf<IViewEngine>();
            using( var stream = new MemoryStream() )
            using( var streamWriter = new StreamWriter( stream, Encoding.UTF8 ) )
            {
                try
                {
                    engine.Render( viewName, layoutName, model, streamWriter );
                }
                catch ( Exception e )
                {
                    streamWriter.Write( helper.RENDER_EXCEPTION_TEMPLATE.AsFormat( e ) );
                    streamWriter.Flush();
                }
                streamWriter.Flush();
                helper.DefineHeaders( x => x.ContentType( ContentType.Html ).ContentLength( stream.Length ) );
                helper.AppendToBody( stream.GetBuffer() );
            }
            return builder;
        }

    }
}