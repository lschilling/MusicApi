using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Xml;
using MusicApi.Models;
using PointW.WebApi.ResourceModel;

namespace MusicApi.Formatters
{
    public class HtmlMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        public bool Indent = false;

        public HtmlMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }

        public override bool CanReadType(Type type)
        {
            return false;
        }

        public override bool CanWriteType(Type type)
        {
            return type.BaseType == typeof (Resource) || type == typeof (IEnumerable<Band>) || type == typeof (IEnumerable<Album>);
            //throw new NotImplementedException();
        }

        /// <summary>The write to stream.</summary>
        /// <param name="type">The type.</param>
        /// <param name="value">The value.</param>
        /// <param name="writeStream">The write stream.</param>
        /// <param name="content">The content.</param>
        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            var settings = new XmlWriterSettings {Indent = Indent, OmitXmlDeclaration = true};
            var writer = XmlWriter.Create(writeStream, settings);
            writer.WriteStartElement("html");
            writer.WriteStartElement("head");
            writer.WriteStartElement("title");
            writer.WriteString("Music API Object");
            writer.WriteEndElement();
            writer.WriteStartElement("style");
            writer.WriteAttributeString("type", "text/css");
            writer.WriteString("td,li {font-family: Tahoma; font-size: 12px;} .name {font-weight: bold; width: 100px;} .album {margin-left: 10px;}");
            writer.WriteEndElement();
            writer.WriteEndElement();

            writer.WriteStartElement("body");

            var bands = value as IEnumerable<Band>;
            if (bands != null)
            {
                foreach (var band in bands)
                    WriteBand(writer, band);
            }
            else
            {
                var band = value as Band;
                if (band != null)
                {
                    WriteBand(writer, band);
                }

                var album = value as Album;
                if (album != null)
                {
                    WriteAlbum(writer, album);
                }
            }
            writer.WriteEndElement(); //body
            writer.WriteEndElement(); //html

            writer.Flush();
            writer.Close();
        }

        private void WriteMetadataRow(XmlWriter writer, string name, string value)
        {
            writer.WriteStartElement("tr");
            writer.WriteStartElement("td");
            writer.WriteAttributeString("class", "name");
            writer.WriteString(name);
            writer.WriteEndElement();
            writer.WriteStartElement("td");
            writer.WriteString(value);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteBand(XmlWriter writer, Band band)
        {
            writer.WriteStartElement("div");
            writer.WriteAttributeString("class", "band");
            writer.WriteAttributeString("id", string.Format("Band_{0}", band.Id));
            writer.WriteStartElement("table");

            WriteMetadataRow(writer, "Id", band.Id.ToString(CultureInfo.InvariantCulture));
            WriteMetadataRow(writer, "Name", band.Name);
            WriteMetadataRow(writer, "Sort", band.SortName);
            WriteMetadataRow(writer, "Genre", band.Genre);

            writer.WriteEndElement();
            foreach (var album in band.Albums)
                WriteAlbum(writer, album);
            writer.WriteEndElement();
        }

        private void WriteAlbum(XmlWriter writer, Album album)
        {
            writer.WriteStartElement("div");
            writer.WriteAttributeString("class", "album");
            writer.WriteAttributeString("id", string.Format("Album_{0}", album.Id));
            writer.WriteStartElement("table");

            WriteMetadataRow(writer, "Album", string.Empty);
            WriteMetadataRow(writer, "Id", album.Id.ToString(CultureInfo.InvariantCulture));
            WriteMetadataRow(writer, "Name", album.Name);
            WriteMetadataRow(writer, "Band", album.BandName);
            WriteMetadataRow(writer, "Genre", album.Genre);

            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteString("Tracks");
            writer.WriteEndElement();
            writer.WriteStartElement("ol");
            foreach (var track in album.Tracks)
                WriteAlbumTrackRow(writer, track.Name, track.Duration.ToString());

            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteAlbumTrackRow(XmlWriter writer, string name, string length)
        {
            writer.WriteStartElement("li");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "track");
            writer.WriteString(name);
            writer.WriteEndElement();
            writer.WriteStartElement("span");
            writer.WriteString(length);
            writer.WriteEndElement();
            writer.WriteEndElement();
        }
    }
}