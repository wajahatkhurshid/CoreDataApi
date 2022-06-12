using Gyldendal.Api.CoreData.Business.Repositories.Common;
using Gyldendal.Api.CoreData.Common.DataProviderInfrastructure;
using Gyldendal.Api.CoreData.DataAccess.KoncernData;
using Gyldendal.APi.CoreData.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gyldendal.Api.CoreData.Business.Provider
{
    public class PressSiteProvider : IPressSiteProvider
    {
        protected readonly koncerndata_webshops_Entities KdEntities;

        public PressSiteProvider(koncerndata_webshops_Entities kdEntities)
        {
            KdEntities = kdEntities;
        }

        public List<Forfatter> GetAuthorsForImport()
        {
            return KdEntities.DEA_KDWS_GDKPresse_Forfatter
                .Select(x => new Forfatter
                {
                    Id = x.forfatter_id,
                    ForfatterNavn = x.forfatter_navn,
                    Fornavn = x.forfatter_fornavn,
                    Efternavn = x.forfatter_efternavn,
                }).ToList();
        }

        public ProductResponse GetBookCovers(BogForsiderRequset requset)
        {
            var result = KdEntities.DEA_KDWS_GDKPresse_Vare
                .Join(KdEntities.DEA_KDWS_GDKPresse_VareKategorier, v => v.vare_id, vk => vk.vare_id,
                    (v, vk) => new { Vare = v, Varekategori = vk })
                .Join(KdEntities.DEA_KDWS_GDKPresse_Kategori, vvk => vvk.Varekategori.kategori_id, k => k.kategori_id,
                    (vvk, k) => new { VareWithKategori = vvk, Kategori = k })
                .Join(KdEntities.workProduct, vwk => vwk.VareWithKategori.Vare.ISBN13, wp => wp.ISBN13,
                    (vwk, wp) => new { VarewKategori = vwk, workProduct = wp })
                .Join(KdEntities.work, wpr => wpr.workProduct.WorkId, w => w.WorkId,
                    (wpr, w) => new { wrkProduct = wpr, Work = w })
                .Where(p => (requset.SearchString == "" ||
                             (p.wrkProduct.VarewKategori.VareWithKategori.Vare.titel.Contains(requset.SearchString) ||
                              p.wrkProduct.VarewKategori.VareWithKategori.Vare.ISBN13.Contains(requset.SearchString)))
                            && (requset.DateFrom == null ||
                                (p.wrkProduct.VarewKategori.VareWithKategori.Vare.udgivelsesdato >= requset.DateFrom &&
                                 p.wrkProduct.VarewKategori.VareWithKategori.Vare.udgivelsesdato <= requset.DateTo))
                            && (requset.Genre == null ||
                                p.wrkProduct.VarewKategori.Kategori.kategori_navn.Contains(requset.Genre))
                            && p.wrkProduct.VarewKategori.VareWithKategori.Vare.Publish2Pressesite == true &&
                            p.wrkProduct.VarewKategori.VareWithKategori.Vare.IsImageReadyInMMS == true)
                .Select(x => new PressSiteProduct
                {
                    VareId = x.wrkProduct.VarewKategori.VareWithKategori.Vare.vare_id,
                    Titel = x.wrkProduct.VarewKategori.VareWithKategori.Vare.titel,
                    PublishDate = x.wrkProduct.VarewKategori.VareWithKategori.Vare.udgivelsesdato,
                    MediaType = x.wrkProduct.VarewKategori.VareWithKategori.Vare.medietype,
                    ImageSource = x.wrkProduct.VarewKategori.VareWithKategori.Vare.forside,
                    ImageSize = 0,
                    Genudg = x.wrkProduct.VarewKategori.VareWithKategori.Vare.udgave > 1 ? "Ja" : "-",
                    Isbn10 = x.wrkProduct.VarewKategori.VareWithKategori.Vare.ostSBN,
                    Isbn13 = x.wrkProduct.VarewKategori.VareWithKategori.Vare.ISBN13,
                    Edition = x.wrkProduct.VarewKategori.VareWithKategori.Vare.udgave,
                    Category = x.wrkProduct.VarewKategori.Kategori.kategori_navn,
                    CategoryOrder = x.wrkProduct.VarewKategori.Kategori.Kategori_order,
                    Organization = x.wrkProduct.VarewKategori.VareWithKategori.Vare.organisationname,
                    WorkId = x.wrkProduct.workProduct.WorkId,
                    //WorkTitle = x.wrkProduct.workProduct.work.Title,
                    WorkTitle = x.Work.Title,
                    TopAuthor = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                        .Join(KdEntities.DEA_KDWS_GDKPresse_VareForfatter, v => v.forfatter_id, vf => vf.forfatter_id,
                            (v, vf) => new { Forfatter = v, VareForfatter = vf })
                        .Where(p => p.VareForfatter.vare_id == x.wrkProduct.VarewKategori.VareWithKategori.Vare.vare_id)
                        .OrderBy(f => f.Forfatter.forfatter_navn).FirstOrDefault().Forfatter.forfatter_navn
                });

            if (requset.SortExpresion.ToLower() == "forfatter")
            {
                result = result.OrderBy(r => r.TopAuthor);
            }
            else if (requset.SortExpresion.ToLower() == "titel")
            {
                result = result.OrderBy(r => r.Titel);
            }
            else if (requset.SortExpresion.ToLower() == "medie")
            {
                result = result.OrderBy(r => r.MediaType);
            }
            else if (requset.SortExpresion.ToLower() == "genre")
            {
                result = result.OrderBy(r => r.Category);
            }
            else if (requset.SortExpresion.ToLower() == "udgivelsesdato")
            {
                result = result.OrderBy(r => r.PublishDate);
            }

            ProductResponse productresponse = new()
            {
                Products = result.Skip((requset.Page - 1) * requset.PageSize).Take(requset.PageSize).ToList(),
                Count = result.Count()
            };

            return productresponse;
        }

        public PressSiteProduct GetBook(string Isbn13)
        {
            var dbProduct = KdEntities.DEA_KDWS_GDKPresse_Vare.FirstOrDefault(b => b.ISBN13.Equals(Isbn13));
            var product = new PressSiteProduct
            {
                VareId = dbProduct.vare_id,
                Titel = dbProduct.titel,
                PublishDate = dbProduct.udgivelsesdato,
                PublishedYear = dbProduct.udgivelsesdato == null ? "" : dbProduct.udgivelsesdato.Value.Year.ToString(),
                MediaType = dbProduct.medietype,
                ImageSource = dbProduct.forside,
                ImageSize = 0,
                Genudg = dbProduct.udgave > 1 ? "Ja" : "-",
                Isbn10 = dbProduct.ostSBN,
                Isbn13 = dbProduct.ISBN13,
                Edition = dbProduct.udgave,
                NumberOfPages = dbProduct.sider,
                Oversatter = dbProduct.Oversaetter,
                Omslagsgrafiker = dbProduct.omslagsgrafiker,
                Description = dbProduct.langbeskrivelse,
                Kontaktperson = dbProduct.kontaktperson_navn,
                KontaktpersonFone = dbProduct.kontaktperson_tlf,
                kontactperosnEmail = dbProduct.kontaktperson_mail,
                Organization = dbProduct.organisationname,
                RetailPrice = dbProduct.vejledende_pris,
                Forfattere = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                    .Join(KdEntities.DEA_KDWS_GDKPresse_VareForfatter, v => v.forfatter_id, vf => vf.forfatter_id,
                        (v, vf) => new { Forfatter = v, VareForfatter = vf })
                    .Where(y => y.VareForfatter.vare_id == dbProduct.vare_id)
                    .Select(x => new Forfatter
                    {
                        ForfatterNavn = x.Forfatter.forfatter_navn,
                        ForfatterProfileLink = x.Forfatter.forfatter_profileLink,
                        IsPhotoExists = !string.IsNullOrEmpty(x.Forfatter.forfatter_foto),
                    }).Distinct().ToList(),
            };
            var authors = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                .Join(KdEntities.DEA_KDWS_GDKPresse_VareForfatter, v => v.forfatter_id, vf => vf.forfatter_id,
                    (v, vf) => new { Forfatter = v, VareForfatter = vf })
                .Where(p => p.VareForfatter.vare_id == product.VareId)
                .OrderBy(f => f.Forfatter.forfatter_navn)
                .Select(y => y.Forfatter.forfatter_navn).Distinct().ToList().AsQueryable();
            product.Authors = string.Join(",", authors);
            return product;
        }

        public List<string> GetCategories()
        {
            return KdEntities.DEA_KDWS_GDKPresse_Kategori.OrderBy(itm => itm.Kategori_order)
                .Select(item => item.kategori_navn).ToList();
        }

        public ForfatterfotoResponse GetAuthorPhotos(AuthorPhotosRequest request)
        {
            var list = new List<Forfatterfoto>();
            int count;
            if (string.IsNullOrEmpty(request.SearchString))
            {
                count = KdEntities.DEA_KDWS_GDKPresse_Forfatter.Count();
            }
            else
            {
                count = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                    .Where(f => f.forfatter_navn.Contains(request.SearchString)).Count();
            }

            if (string.IsNullOrEmpty(request.SearchString))
            {
                list = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                    .OrderBy(item => item.forfatter_navn)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(forfatter => new Forfatterfoto
                    {
                        Forfatter = forfatter.forfatter_navn,
                        ImageSource = forfatter.forfatter_foto,
                        Ar = "NAV",
                        Fotograf = "NAV",
                        Kommentar = "NAV",
                    }).ToList();
            }
            else
            {
                list = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                    .Where(f => f.forfatter_navn.Contains(request.SearchString))
                    .OrderBy(item => item.forfatter_navn)
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .Select(forfatter => new Forfatterfoto
                    {
                        Forfatter = forfatter.forfatter_navn,
                        ImageSource = forfatter.forfatter_foto,
                    }).ToList();
            }

            ForfatterfotoResponse forfatterfotoResponse = new()
            {
                Forfatterfoto = list,
                Count = count
            };
            return forfatterfotoResponse;
        }

        public ForfatterResponse GetAuthors(AuthorsRequest request)
        {
            var optionRecompileInterceptor = new OptionRecompileInterceptor();
            DbInterception.Add(optionRecompileInterceptor);

            var query = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                .Join(KdEntities.DEA_KDWS_GDKPresse_Materiale, f => f.forfatter_id, m => m.forfatter_id,
                    (f, m) => new { Forfatter = f, Material = m });
            
            if (!string.IsNullOrWhiteSpace(request.SearchString))
                query = query.Where(f => f.Forfatter.forfatter_navn.Contains(request.SearchString));

            query = query
                .OrderBy(f => f.Forfatter.forfatter_efternavn)
                .AsNoTracking();

            var result = query
                .Select(f => new Forfatter
                {
                    ForfatterNavn = f.Forfatter.forfatter_navn,
                    ForfatterProfileLink = f.Forfatter.forfatter_profileLink,
                    Photo = new Forfatterfoto
                    {
                        Fotograf = f.Material.materiale_fotograf,
                        ImageSource = f.Material.materiale_foto_link ?? string.Empty,
                        ImageSize = f.Material.materiale_size,
                        Kommentar = f.Material.materiale_kommentar,
                        Ar = f.Material.materiale_aarstal,
                        Thumbnail = f.Material.materiale_foto_thumbnail,
                    }
                })
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).ToList();

            ForfatterResponse forfatterResponse = new()
            {
                Forfatter = result,
                Count = query.Count()
            };

            DbInterception.Remove(optionRecompileInterceptor); //Remove interceptor to not affect other queries
            return forfatterResponse;
        }

        public ProductResponse GetUpcomingTitles(UpcomingTitlesRequest requset)
        {
            if (requset.Genre != null)
            {
                if (requset.Genre.ToLower() == "alle")
                    requset.Genre = null;
            }

            var result = KdEntities.DEA_KDWS_GDKPresse_Vare
                .Join(KdEntities.DEA_KDWS_GDKPresse_VareKategorier, v => v.vare_id, vk => vk.vare_id,
                    (v, vk) => new { Vare = v, Varekategori = vk })
                .Join(KdEntities.DEA_KDWS_GDKPresse_Kategori, vvk => vvk.Varekategori.kategori_id, k => k.kategori_id,
                    (vvk, k) => new { VareWithKategori = vvk, Kategori = k })
                .Where(p => (requset.SearchString == "" || requset.SearchString == null ||
                             (p.VareWithKategori.Vare.titel.Contains(requset.SearchString) ||
                              p.VareWithKategori.Vare.ISBN13.Contains(requset.SearchString)))
                            && (requset.DateFrom == null ||
                                (p.VareWithKategori.Vare.udgivelsesdato >= requset.DateFrom &&
                                 p.VareWithKategori.Vare.udgivelsesdato <= requset.DateTo))
                            && (requset.Genre == null || p.Kategori.kategori_navn.Contains(requset.Genre)) &&
                            p.VareWithKategori.Vare.IsNewTitle == true)
                .Select(x => new
                {
                    VareId = x.VareWithKategori.Vare.vare_id,
                    Titel = x.VareWithKategori.Vare.titel,
                    PublishDate = x.VareWithKategori.Vare.udgivelsesdato,
                    MediaType = x.VareWithKategori.Vare.medietype,
                    ImageSource = x.VareWithKategori.Vare.forside,
                    ImageSize = 0,
                    Genudg = x.VareWithKategori.Vare.udgave > 1 ? "Ja" : "-",
                    Isbn10 = x.VareWithKategori.Vare.ostSBN,
                    Isbn13 = x.VareWithKategori.Vare.ISBN13,
                    Edition = x.VareWithKategori.Vare.udgave,
                    Category = x.Kategori.kategori_navn,
                    CategoryOrder = x.Kategori.Kategori_order,
                    Organization = x.VareWithKategori.Vare.organisationname,
                    AuthorsList = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                        .Join(KdEntities.DEA_KDWS_GDKPresse_VareForfatter, v => v.forfatter_id, vf => vf.forfatter_id,
                            (v, vf) => new { Forfatter = v, VareForfatter = vf })
                        .Where(p => p.VareForfatter.vare_id == x.VareWithKategori.Vare.vare_id)
                        .OrderBy(f => f.Forfatter.forfatter_navn)
                        .Select(y => y.Forfatter.forfatter_navn).Distinct(),
                });
            if (requset.SortExpresion.ToLower() == "titel")
            {
                result = result.OrderBy(r => r.Titel);
            }
            else if (requset.SortExpresion.ToLower() == "medie")
            {
                result = result.OrderBy(r => r.MediaType);
            }
            else if (requset.SortExpresion.ToLower() == "genre")
            {
                result = result.OrderBy(r => r.Category);
            }
            else if (requset.SortExpresion.ToLower() == "udgivelsesdato")
            {
                result = result.OrderByDescending(r => r.PublishDate);
            }

            var products = result.Skip((requset.Page - 1) * requset.PageSize).Take(requset.PageSize).ToList().Select(
                x => new PressSiteProduct
                {
                    VareId = x.VareId,
                    Titel = x.Titel,
                    PublishDate = x.PublishDate,
                    MediaType = x.MediaType,
                    ImageSource = x.ImageSource,
                    ImageSize = x.ImageSize,
                    Genudg = x.Genudg,
                    Isbn10 = x.Isbn10,
                    Isbn13 = x.Isbn13,
                    Edition = x.Edition,
                    Category = x.Category,
                    CategoryOrder = x.CategoryOrder,
                    Organization = x.Organization,
                    Authors = string.Join(",", x.AuthorsList.ToArray())
                }).ToList();

            ProductResponse productresponse = new()
            {
                Products = products,
                Count = result.Count()
            };
            return productresponse;
        }

        public List<PressSiteProduct> GetNewsletterTitles(NewsletterRequest request)
        {
            var result = KdEntities.DEA_KDWS_GDKPresse_Vare
                .Join(KdEntities.DEA_KDWS_GDKPresse_VareKategorier, v => v.vare_id, vk => vk.vare_id,
                    (v, vk) => new { Vare = v, Varekategori = vk })
                .Join(KdEntities.DEA_KDWS_GDKPresse_Kategori, vvk => vvk.Varekategori.kategori_id, k => k.kategori_id,
                    (vvk, k) => new { VareWithKategori = vvk, Kategori = k })
                .Where(p => (request.DateFrom == null || (p.VareWithKategori.Vare.udgivelsesdato >= request.DateFrom &&
                                                          p.VareWithKategori.Vare.udgivelsesdato <= request.DateTo))
                            && (request.Options == 1 ||
                                (request.Options == 0 && request.Genre.Contains(p.Kategori.kategori_navn)))
                            && (request.Options == 0 ||
                                (request.Options == 1 && !request.Genre.Contains(p.Kategori.kategori_navn))))
                .Select(x => new
                {
                    VareId = x.VareWithKategori.Vare.vare_id,
                    Titel = x.VareWithKategori.Vare.titel,
                    Subtitle = x.VareWithKategori.Vare.undertitel,
                    PublishDate = x.VareWithKategori.Vare.udgivelsesdato,
                    MediaType = x.VareWithKategori.Vare.medietype,
                    ImageSource = x.VareWithKategori.Vare.forside,
                    ImageSize = 0,
                    Genudg = x.VareWithKategori.Vare.udgave > 1 ? "Ja" : "-",
                    Isbn10 = x.VareWithKategori.Vare.ostSBN,
                    Isbn13 = x.VareWithKategori.Vare.ISBN13,
                    Edition = x.VareWithKategori.Vare.udgave,
                    Category = x.Kategori.kategori_navn,
                    CategoryOrder = x.Kategori.Kategori_order,
                    Organization = x.VareWithKategori.Vare.organisationname,
                    AuthorList = KdEntities.DEA_KDWS_GDKPresse_Forfatter
                        .Join(KdEntities.DEA_KDWS_GDKPresse_VareForfatter, v => v.forfatter_id, vf => vf.forfatter_id,
                            (v, vf) => new { Forfatter = v, VareForfatter = vf })
                        .Where(p => p.VareForfatter.vare_id == x.VareWithKategori.Vare.vare_id)
                        .OrderBy(f => f.Forfatter.forfatter_navn)
                        .Select(y => y.Forfatter.forfatter_navn).Distinct()
                });
            var products = result.OrderBy(x => x.Category).ToList().Select(x => new PressSiteProduct
            {
                VareId = x.VareId,
                Titel = x.Titel,
                Subtitle = x.Subtitle,
                PublishDate = x.PublishDate,
                MediaType = x.MediaType,
                ImageSource = x.ImageSource,
                ImageSize = x.ImageSize,
                Genudg = x.Edition > 1 ? "Ja" : "-",
                Isbn10 = x.Isbn10,
                Isbn13 = x.Isbn13,
                Edition = x.Edition,
                Category = x.Category,
                CategoryOrder = x.CategoryOrder,
                Organization = x.Organization,
                Authors = string.Join(", ", x.AuthorList.ToArray())
            }).ToList();
            return products;
        }

        public List<PressSiteProduct> GetProductsForImport(ProductImportRequest request)
        {
            return KdEntities.DEA_KDWS_GDKPresse_Vare.Where
            (
                book => book.Publish2Pressesite == true
                        &&
                        book.udgivelsesdato >= request.DateFrom
                        &&
                        book.udgivelsesdato <= request.DateTo
            ).Select(x => new PressSiteProduct
            {
                Isbn13 = x.ISBN13,
                Titel = x.titel
            }).ToList();
        }

        public TypeAheadResponse GetTypeAheadResults(TypeAheadRequest typeahedrequest)
        {
            TypeAheadResponse typeaheadresponse = new();

            AuthorsRequest authorrequest = new()
            {
                SearchString = typeahedrequest.PrefixText,
                Page = 1,
                PageSize = 5,
                SortExpresion = "Forfatter"
            };
            var forfatters = GetAuthors(authorrequest);
            typeaheadresponse.CountAuthors = forfatters.Count;
            typeaheadresponse.ListAuthors = forfatters.Forfatter.Select(x => new Forfatterfoto
            {
                Forfatter = x.ForfatterNavn,
                ImageSource = x.Photo.ImageSource
            }).ToList();
            UpcomingTitlesRequest request = new()
            {
                DateFrom = typeahedrequest.FromDateK,
                DateTo = typeahedrequest.ToDateK,
                SearchString = typeahedrequest.PrefixText,
                Genre = null,
                Page = 1,
                PageSize = 5,
                SortExpresion = "Titel"
            };
            var titles = GetUpcomingTitles(request);
            typeaheadresponse.CountKtitler = titles.Count;
            typeaheadresponse.ListKtitler = titles.Products.Select(x => new BogForsider
            {
                Titel = x.Titel,
                OstISBN = x.Isbn10,
                Organization = x.Organization,
                ISBN13 = x.Isbn13,
                PublishDate = x.PublishDate.Value
            }).ToList();
            typeaheadresponse.CountBooks = KdEntities.DEA_KDWS_GDKPresse_Vare.Count(item =>
                (item.titel.Contains(typeahedrequest.PrefixText) || item.ISBN13.Contains(typeahedrequest.PrefixText))
                && item.Publish2Pressesite == true
                && item.IsImageReadyInMMS == true
                && item.udgivelsesdato >= typeahedrequest.FromDate
                && item.udgivelsesdato <= typeahedrequest.ToDate);

            typeaheadresponse.ListBooks = KdEntities.DEA_KDWS_GDKPresse_Vare
                .Where(b => (b.titel.Contains(typeahedrequest.PrefixText) ||
                             b.ISBN13.Contains(typeahedrequest.PrefixText))
                            && b.Publish2Pressesite == true
                            && b.IsImageReadyInMMS == true
                            && b.udgivelsesdato >= typeahedrequest.FromDate
                            && b.udgivelsesdato <= typeahedrequest.ToDate)
                .Select(b => new BogForsider
                {
                    Titel = b.titel,
                    BookDetailLink = b.ISBN13,
                    ISBN13 = b.ISBN13,
                    OstISBN = b.ostSBN,
                    Organization = b.organisationname,
                    PublishDate = (b.udgivelsesdato.HasValue) ? b.udgivelsesdato.Value : DateTime.Now
                }).Take(5).ToList();
            return typeaheadresponse;
        }
    }

    public class OptionRecompileInterceptor : DbCommandInterceptor
    {
        static void AddOptionToCommand(DbCommand command)
        {
            string optionRecompileString = "\r\nOPTION (RECOMPILE)";
            if (!command.CommandText.Contains(optionRecompileString)) //Check the option is not added already
            {
                command.CommandText += optionRecompileString;
            }
        }


        public override void NonQueryExecuting(
            DbCommand command, DbCommandInterceptionContext<int> interceptionContext)
        {
            AddOptionToCommand(command);
        }

        public override void ReaderExecuting(
            DbCommand command, DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            AddOptionToCommand(command);
        }

        public override void ScalarExecuting(
            DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            AddOptionToCommand(command);
        }
    }
}