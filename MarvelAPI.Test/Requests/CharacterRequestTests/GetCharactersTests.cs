﻿using MarvelAPI.Parameters;
using Moq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarvelAPI.Test.Requests.CharacterRequestTests
{
    public class GetCharactersTests : CharacterRequestTestBase
    {
        [Fact]
        public void Success()
        {
            // arrange
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c => c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => r.Resource == "/characters")))
                .Returns(new RestResponse<Wrapper<Character>>
                {
                    Data = new Wrapper<Character>
                    {
                        Data = new Container<Character>
                        {
                            Results = characterList
                        }
                    }
                })
                .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {

            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Name_Search()
        {
            // arrange
            var searchTerm = "searchTerm";
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c => 
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r => 
                    r.Parameters.Any(p => 
                        p.Name == "name" && p.Value.ToString() == searchTerm
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Name = searchTerm
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void NameStartsWith_Search()
        {
            // arrange
            var searchTerm = "searchTerm";
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "nameStartsWith" && p.Value.ToString() == searchTerm
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                NameStartsWith = searchTerm
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void ModifiedSince_Search()
        {
            // arrange
            var dateSearch = DateTime.UtcNow;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "modifiedSince" && dateSearch.ToString("yyyy-MM-dd") == p.Value.ToString()
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                ModifiedSince = dateSearch
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Comics_Search()
        {
            // arrange
            var searchList = new List<int> { 1, 2, 3 };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "comics" && p.Value.ToString() == string.Join(",", searchList)
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Comics = searchList
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Series_Search()
        {
            // arrange
            var searchList = new List<int> { 1, 2, 3 };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "series" && p.Value.ToString() == string.Join(",", searchList)
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Series = searchList
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Events_Search()
        {
            // arrange
            var searchList = new List<int> { 1, 2, 3 };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "events" && p.Value.ToString() == string.Join(",", searchList)
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Events = searchList
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Stories_Search()
        {
            // arrange
            var searchList = new List<int> { 1, 2, 3 };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "stories" && p.Value.ToString() == string.Join(",", searchList)
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Stories = searchList
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void OrderBy_Single()
        {
            // arrange
            var orderBy = new List<OrderBy> { OrderBy.Name };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "orderBy" && p.Value.ToString() == string.Join(",", orderBy.Select(o => o.ToParameter()))
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Order = orderBy
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void OrderBy_ExcludeNonAvailable()
        {
            // arrange
            var orderBy = new List<OrderBy> { OrderBy.Name, OrderBy.FirstName };
            var availableOrderBy = new List<OrderBy> { OrderBy.Name };
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "orderBy" && p.Value.ToString() == string.Join(",", availableOrderBy.Select(a => a.ToParameter()))
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Order = orderBy
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Limit_Search()
        {
            // arrange
            var limit = 1;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "limit" && p.Value.ToString() == limit.ToString()
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Limit = limit
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }

        [Fact]
        public void Offset_Search()
        {
            // arrange
            var offset = 1;
            var characterList = new List<Character>
            {
                new Character
                {
                }
            };

            RestClientMock.Setup(c =>
                c.Execute<Wrapper<Character>>(It.Is<IRestRequest>(r =>
                    r.Parameters.Any(p =>
                        p.Name == "offset" && p.Value.ToString() == offset.ToString()
            ))))
            .Returns(new RestResponse<Wrapper<Character>>
            {
                Data = new Wrapper<Character>
                {
                    Data = new Container<Character>
                    {
                        Results = characterList
                    }
                }
            })
            .Verifiable();

            // act
            var characters = Requests.GetCharacters(new GetCharacters
            {
                Offset = offset
            });

            // assert
            Assert.Equal(characterList.Count, characters.Count());
            RestClientMock.VerifyAll();
        }
    }
}
