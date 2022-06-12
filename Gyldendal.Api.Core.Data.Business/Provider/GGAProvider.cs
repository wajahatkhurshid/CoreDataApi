using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Business.Provider
{
    public class GGAProvider : IGGAProvider
    {
        protected readonly koncerndata_webshops_Entities KdEntities;
        public GGAProvider(koncerndata_webshops_Entities kdEntities)
        {
            KdEntities = kdEntities;
        }
        public SearchDtoResponse Search(string criteria, int mode)
        {
            var searchResult = new SearchDtoResponse();
                    if (mode == (int)SearchType.Title)//Search Title with its Authors
                    {
                        searchResult =
                            KdEntities.DEA_KDWS_GGA_Vare
                                .Where(c => c.ISBN13 == criteria).Select(r => new SearchDtoResponse
                                {
                                    ISBN = r.ISBN13,
                                    TitleName = r.titel,
                                    EditionNumber = r.udgave,
                                    Illustrator = r.illustrator,
                                    Translator = r.translator,
                                    OriginalPublisher = r.Publisher,
                                    Pages = r.No_of_Pages,
                                    PublicationDate = r.udgivelsesdato
                                }).FirstOrDefault();
                        if (searchResult == null) return null;
                        var authors =
                            KdEntities.DEA_KDWS_GGA_Forfatter.Join(KdEntities.DEA_KDWS_GGA_Vare_Forfatter, a => a.forfatter_id,
                                b => b.forfatter_id, (a, b) => new { A = a, B = b })
                                .Where(c => c.B.vare_id == searchResult.ISBN).Select(r => new AuthorResponse
                                {
                                    AuthorName = r.A.forfatter_navn,
                                    FirstName = r.A.forfatter_fornavn,
                                    LastName = r.A.forfatter_efternavn,
                                    AuthorUrlBig = r.A.forfatter_profileLink_Large,
                                    AuthorUrlMed = r.A.forfatter_profileLink_Medium,
                                    AuthorUrlSmall = r.A.forfatter_profileLink_Small,
                                    AuthorId = r.A.forfatter_id
                                }).ToList();
                        searchResult.Authors = authors;
                    }
                    else//Search Authors
                    {
                        var authors =
                               KdEntities.DEA_KDWS_GGA_Forfatter.Where(c => c.forfatter_navn.Contains(criteria)).Select(r => new AuthorResponse
                               {
                                   AuthorName = r.forfatter_navn,
                                   FirstName = r.forfatter_fornavn,
                                   LastName = r.forfatter_efternavn,
                                   AuthorUrlBig = r.forfatter_profileLink_Large,
                                   AuthorUrlMed = r.forfatter_profileLink_Medium,
                                   AuthorUrlSmall = r.forfatter_profileLink_Small,
                                   AuthorId = r.forfatter_id
                               }).ToList();
                        searchResult.Authors = authors;
                    }
                    return searchResult;   
        }
    }
}
