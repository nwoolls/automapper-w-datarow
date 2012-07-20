using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WindowsFormsApplication5
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            using (DestinationDataTableAdapters.PersonTableAdapter destinationAdapter = new DestinationDataTableAdapters.PersonTableAdapter())
            {
                using (SourceDataTableAdapters.PersonTableAdapter sourceAdapter = new SourceDataTableAdapters.PersonTableAdapter())
                {
                    using (SourceData.PersonDataTable sourceDataTable = sourceAdapter.GetData())
                    {
                        using (DestinationData.PersonDataTable destinationDataTable = new DestinationData.PersonDataTable())
                        {
                            SourceData.PersonRow sourceRow = sourceDataTable.FirstOrDefault();
                            if (sourceRow != null)
                            {
                                TestMappingToDataRow(destinationDataTable, sourceRow);

                                TestMappingToPoco(sourceRow);
                            }
                        }
                    }
                }
            }
        }

        private static void TestMappingToDataRow(DestinationData.PersonDataTable destinationDataTable, SourceData.PersonRow sourceRow)
        {
            DestinationData.PersonRow destinationRow = destinationDataTable.NewPersonRow();

            AutoMapper.Mapper.CreateMap<SourceData.PersonRow, DestinationData.PersonRow>()

                //comment out the .FormMember()'s below and receive DBNull errors, so this IS working to some degree, plus works with POCO
                .ForMember(m => m.Title, opt => opt.MapFrom<string>(src => src.IsTitleNull() ? "adsf" : src.Title))
                .ForMember(m => m.Suffix, opt => opt.MapFrom<string>(src => src.IsSuffixNull() ? "adsf" : src.Suffix))
                .ForMember(m => m.AdditionalContactInfo, opt => opt.MapFrom<string>(src => src.IsAdditionalContactInfoNull() ? "adsf" : src.AdditionalContactInfo))

                //with this ForMember, every FirstName should be "First!" - again works with POCO not DataRow
                .ForMember(m => m.FirstName, opt => opt.MapFrom<string>(src => "First!"));


            AutoMapper.Mapper.Map<SourceData.PersonRow, DestinationData.PersonRow>(sourceRow, destinationRow);
            //mapping succesful, destinationRow.Title & Suffix SHOULD be "asdf" - they aren't
            if (destinationRow.IsTitleNull())
            {
                MessageBox.Show("why :(");
            }

            //every FirstName should be "First!"
            //in fact, if you put a BP in DestionationData.Designer.cs in the setter for Person.FirstName
            //you will clearly see it is called only once, and with First!
            //so why does the value end up being the value for FirstName from sourceRow instead?
            if (!destinationRow.FirstName.Equals("First!"))
            {
                MessageBox.Show("why! :(");
            }

            //the result seems to be that sourceRow is copied directly to destinationRow somehow, ignoring the MapFrom()'s

        }

        private static void TestMappingToPoco(SourceData.PersonRow sourceRow)
        {
            //now the Poco - this all works
            AutoMapper.Mapper.CreateMap<SourceData.PersonRow, PocoPerson>()
                .ForMember(m => m.Title, opt => opt.MapFrom(src => src.IsTitleNull() ? "asdf" : src.Title))
                .ForMember(m => m.Suffix, opt => opt.MapFrom(src => src.IsSuffixNull() ? "asdf" : src.Suffix))
                .ForMember(m => m.FirstName, opt => opt.MapFrom(src => "First!"));

            PocoPerson pocoPerson = new PocoPerson();

            AutoMapper.Mapper.Map<SourceData.PersonRow, PocoPerson>(sourceRow, pocoPerson);
            if (pocoPerson.Title.Equals("asdf"))
            {
                MessageBox.Show("poco works");
            }

            if (pocoPerson.FirstName.Equals("First!"))
            {
                MessageBox.Show("poco works again");
            }
        }
    }
}
