using MarvelAPI.Parameters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MSTestExtensions;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    [TestClass]
    public class GetComicsForCharacterTests : CharacterRequestTestBase
    {
        [TestMethod]
        public void Success()
        {
            // arrange
            var characterId = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r => r.Resource == string.Format($"/characters/{characterId}/comics"))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void Format_Filter()
        {
            // arrange
            var characterId = 1;
            var format = ComicFormat.Comic;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r => 
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "format" && p.Value.ToString() == format.ToParameter()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                Format = format
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void FormatType_Filter()
        {
            // arrange
            var characterId = 1;
            var formatType = ComicFormatType.Comic;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "formatType" && p.Value.ToString() == formatType.ToParameter()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                FormatType = formatType
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void NoVariant_Filter()
        {
            // arrange
            var characterId = 1;
            var noVariant = true;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "noVariants" && p.Value.ToString() == noVariant.ToString().ToLower()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                NoVariants = noVariant
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void DateDescriptor_Filter()
        {
            // arrange
            var characterId = 1;
            var dateDescriptor = DateDescriptor.LastWeek;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "dateDescriptor" && p.Value.ToString() == dateDescriptor.ToParameter()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                DateDescript = dateDescriptor
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void DateRange_Filter()
        {
            // arrange
            var characterId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = startDate.AddDays(7);
            var rangeString = string.Format("{0},{1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "dateRange" && p.Value.ToString() == rangeString))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                DateRangeBegin = startDate,
                DateRangeEnd = endDate
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void DateRange_Invalid()
        {
            // arrange
            var characterId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = startDate.AddDays(-1);
            var rangeString = string.Format("{0},{1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "dateRange" && p.Value.ToString() == rangeString))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            Assert.Throws<ArgumentException>(() => Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                DateRangeBegin = startDate,
                DateRangeEnd = endDate
            }));
        }

        [TestMethod]
        public void DateRange_MissingOne()
        {
            // arrange
            var characterId = 1;
            var startDate = DateTime.Now.AddMonths(-1);
            var endDate = startDate.AddDays(1);
            var rangeString = string.Format("{0},{1}", startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "dateRange" && p.Value.ToString() == rangeString))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            Assert.Throws<ArgumentException>(() => Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                DateRangeBegin = startDate
            }));
        }

        [TestMethod]
        public void HasDigitalIssue_Filter()
        {
            // arrange
            var characterId = 1;
            var hasDigitalIssue = true;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "hasDigitalIssue" && p.Value.ToString() == hasDigitalIssue.ToString().ToLower()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                HasDigitalIssue = hasDigitalIssue
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void ModifiedSince_Filter()
        {
            // arrange
            var characterId = 1;
            var modifiedSince = DateTime.Now.AddMonths(-1);
            var modifiedSinceString = modifiedSince.ToString("yyyy-MM-dd");
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "modifiedSince" && p.Value.ToString() == modifiedSinceString))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                ModifiedSince = modifiedSince
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void Series_Filter()
        {
            // arrange
            var characterId = 1;
            var searchList = new List<int> { 1, 2, 3 };
            var comicList = new List<Comic>
            {
                new Comic { }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Comic>>(It.Is<IRestRequest>(r =>
                    r.Resource == string.Format($"/characters/{characterId}/comics")
                    && r.Parameters.Any(p =>
                        p.Name == "series" && p.Value.ToString() == string.Join(",", searchList)
            ))))
            .Returns(new RestResponse<Wrapper<Comic>>
            {
                Data = new Wrapper<Comic>
                {
                    Data = new Container<Comic>
                    {
                        Results = comicList
                    }
                }
            })
            .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                Series = searchList
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void Limit_Filter()
        {
            // arrange
            var characterId = 1;
            var limit = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "limit" && p.Value.ToString() == limit.ToString()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                Limit = limit
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }

        [TestMethod]
        public void Offset_Filter()
        {
            // arrange
            var characterId = 1;
            var offset = 1;
            var comicList = new List<Comic>
            {
                new Comic { }
            };
            RestClientMock.Setup(c => c.Execute<Wrapper<Comic>>(
                It.Is<IRestRequest>(r =>
                r.Resource == string.Format($"/characters/{characterId}/comics")
                && r.Parameters.Any(p => p.Name == "offset" && p.Value.ToString() == offset.ToString()))))
                .Returns(new RestResponse<Wrapper<Comic>>
                {
                    Data = new Wrapper<Comic>
                    {
                        Data = new Container<Comic>
                        {
                            Results = comicList
                        }
                    }
                })
                .Verifiable();

            // act
            var comics = Requests.GetComicsForCharacter(new GetComicsForCharacter
            {
                CharacterId = characterId,
                Offset = offset
            });

            // assert
            Assert.AreEqual(comicList.Count, comics.Count());
            RestClientMock.VerifyAll();
        }
    }
}
