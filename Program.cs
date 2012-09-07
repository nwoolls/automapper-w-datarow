using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    class Program
    {
        static void Main(string[] args)
        {
            SourceData.PersonDataTable sourceDataTable = new SourceData.PersonDataTable();

            SourceData.PersonRow sourceRow = sourceDataTable.NewPersonRow();

            //initialize row
            sourceRow.BusinessEntityID = 0;
            sourceRow.PersonType = "";
            sourceRow.NameStyle = false;
            sourceRow.MiddleName = "";
            sourceRow.LastName = "";
            sourceRow.Title = "";
            sourceRow.Suffix = "";
            sourceRow.FirstName = "";
            sourceRow.EmailPromotion = 0;
            sourceRow.Demographics = "";
            sourceRow.rowguid = Guid.Empty;
            sourceRow.ModifiedDate = DateTime.MinValue;
            sourceRow.AdditionalContactInfo = "";

            if (sourceRow != null)
            {
                TestMappingToDataRow(sourceRow);

                TestMappingToPoco(sourceRow);
            }
        }

        private static void TestMappingToDataRow(SourceData.PersonRow sourceRow)
        {
            DestinationData.PersonDataTable destinationDataTable = new DestinationData.PersonDataTable();
            DestinationData.PersonRow destinationRow = destinationDataTable.NewPersonRow();

            AutoMapper.Mapper.CreateMap<SourceData.PersonRow, DestinationData.PersonRow>()
                //with this ForMember, every FirstName should be "First!"
                .ForMember(src => src.FirstName, opt => opt.MapFrom<string>(src => "First!"))

                //don't map properties introduced in DataRow
                .ForMember(src => src.ItemArray, opt => opt.Ignore())
                ;

            AutoMapper.Mapper.Map<SourceData.PersonRow, DestinationData.PersonRow>(sourceRow, destinationRow);

            //this assertion will fail unless you use opt.Ignore() for ItemArray
            Debug.Assert(destinationRow.FirstName.Equals("First!"), "datarow mapping failed");

        }

        private static void TestMappingToPoco(SourceData.PersonRow sourceRow)
        {
            PocoPerson pocoPerson = new PocoPerson();

            //now the Poco - this all works
            AutoMapper.Mapper.CreateMap<SourceData.PersonRow, PocoPerson>()
                .ForMember(src => src.FirstName, opt => opt.MapFrom(src => "First!"));

            AutoMapper.Mapper.Map<SourceData.PersonRow, PocoPerson>(sourceRow, pocoPerson);

            Debug.Assert(pocoPerson.FirstName.Equals("First!"), "poco mapping failed");
        }
    }
}
