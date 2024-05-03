using AutoMapper;
using Cosmos.Edison.NetCore.Business.Services;
using Cosmos.Edison.NetCore.Controllers;
using Cosmos.Edison.NetCore.Models.Document;
using Moq;
using Cosmos.Edison.NetCore.Business.Dto.Document;
using Cosmos.Edison.NetCore.Business.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Cosmos.Edison.NetCore.Tests.ControllerTests
{
    [TestFixture]
    public class DocumentTypesControllerTests
    {
        private DocumentTypesController _controller;
        private Mock<DocumentTypeService> mockDocumentTypeService;
        private Mock<DocumentIndexInfoService> mockDocumentIndexInfoService;
        private Mock<DocTypeIndexService> mockDocTypeIndexService;
        private Mock<DocumentTypeGroupService> mockDocumentTypeGroupService;

        // Mapper
        private static readonly MapperConfiguration mappingConfig = new(mc =>
        {
            mc.AddProfile(new DocumentTypeProfile());
            mc.AddProfile(new IndexInfoProfile());
            mc.AddProfile(new DocumentTypeDtoProfile());
            mc.AddProfile(new DocTypeIndexDtoProfile());
            mc.AddProfile(new ResponseDocumentTypesProfile());
            mc.AddProfile(new DocumentTypeGroupDtoProfile());
            mc.AddProfile(new RequestCreateDocumentTypeProfile());
            mc.AddProfile(new RequestUpdateDocumentTypeProfile());
            
        });
        private static readonly IMapper mapper = mappingConfig.CreateMapper();


        [SetUp]
        public void Setup()
        {
            mockDocumentTypeService = new Mock<DocumentTypeService>();
            mockDocumentIndexInfoService = new Mock<DocumentIndexInfoService>();
            mockDocTypeIndexService = new Mock<DocTypeIndexService>();
            mockDocumentTypeGroupService = new Mock<DocumentTypeGroupService>();

            _controller = new DocumentTypesController(mockDocumentTypeService.Object, mockDocumentIndexInfoService.Object, mockDocTypeIndexService.Object,
                mockDocumentTypeGroupService.Object, mapper);
        }

        [Test]
        public async Task Get_Document_Type()
        {
            DocumentTypeDto documentTypeDto = new DocumentTypeDto
            {
                Id = 1,
                Name = "Test",
            };


            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                new DocTypeIndexDto { DocumentTypeId = 1, IndexId = 2, Required = true }
            };

            IndexInfoDto indexInfoDto = new IndexInfoDto
            {
                Name = "Test"
            };

            mockDocumentTypeService.Setup(_ => _.GetActiveByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeDto);
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(indexInfoDto);

            string shortName = "shortName";

            var result = await _controller.GetDocumentType(shortName, "", "");
            var actual = (result.Result as ObjectResult).Value as DocumentType;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IndexInfoList.First().Name, Is.EqualTo("Test"));
        }

        [Test]
        public async Task Get_Document_Type_Continue_Case()
        {
            DocumentTypeDto documentTypeDto = new DocumentTypeDto
            {
                Id = 1,
                Name = "Test",
            };


            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                new DocTypeIndexDto { DocumentTypeId = 1, IndexId = 2, Required = true }
            };

            IndexInfoDto? indexInfoDto = null;
            

            mockDocumentTypeService.Setup(_ => _.GetActiveByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeDto);
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(indexInfoDto);

            string shortName = "shortName";

            var result = await _controller.GetDocumentType(shortName, "", "");
            var actual = (result.Result as ObjectResult).Value as DocumentType;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.IndexInfoList, Is.Empty);
        }

        [Test]
        public async Task Get_Document_Types()
        {
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test1" },
                new DocumentTypeDto { Id = 2, Name = "Test2" },
            };

            mockDocumentTypeService.Setup(_ => _.GetAllAsync(Status.ACTIVE)).ReturnsAsync(documentTypeDtos);

            long id = 0;
            var result = await _controller.GetDocumentTypes(id, "", "");
            var actual = (result.Result as ObjectResult).Value as List<DocumentType>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<List<DocumentType>>());
            Assert.That(actual, Has.Count.EqualTo(2));
        }

        [Test]
        public async Task Get_Document_Types_By_Group()
        {
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test1" },
                new DocumentTypeDto { Id = 2, Name = "Test2" },
            };

            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                new DocTypeIndexDto { DocumentTypeId = 1, IndexId = 1, Required = true },
                new DocTypeIndexDto { DocumentTypeId = 2, IndexId = 2, Required = false },
            };

            DocumentTypeGroupDto documentTypeGroupDto = new DocumentTypeGroupDto();
            IndexInfoDto ındexInfoDto = new IndexInfoDto();

            mockDocumentTypeGroupService.Setup(_ => _.GetByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeGroupDto);
            mockDocumentTypeService.Setup(_ => _.GetByGroupIdAsync(It.IsAny<long>())).ReturnsAsync(documentTypeDtos);
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(ındexInfoDto);

            string shortname = "test shortname";
            var result = await _controller.GetDocumentTypesByGroup(shortname, "", "");
            var actual = (result.Result as ObjectResult).Value as List<DocumentType>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<List<DocumentType>>());
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual.First().IndexInfoList.First().Required, Is.EqualTo(true));
        }

        [Test]
        public async Task Get_Document_Types_By_Group_Continue_Case_For_Null_IndexInfoDto()
        {
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test1" },
            };

            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                new DocTypeIndexDto { DocumentTypeId = 1, IndexId = 1, Required = true },
            };

            DocumentTypeGroupDto documentTypeGroupDto = new DocumentTypeGroupDto();
            IndexInfoDto? ındexInfoDto = null;

            mockDocumentTypeGroupService.Setup(_ => _.GetByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeGroupDto);
            mockDocumentTypeService.Setup(_ => _.GetByGroupIdAsync(It.IsAny<long>())).ReturnsAsync(documentTypeDtos);
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(ındexInfoDto);

            string shortname = "test shortname";
            var result = await _controller.GetDocumentTypesByGroup(shortname, "", "");
            var actual = (result.Result as ObjectResult).Value as List<DocumentType>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<List<DocumentType>>());
            Assert.That(actual, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task Get_Document_Types_By_Group_Continue_Case()
        {
            DocumentTypeDto? dto = null;
            List<DocumentTypeDto> documentTypeDtos = new()
            {
                dto
            };

            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                new DocTypeIndexDto { DocumentTypeId = 1, IndexId = 1, Required = true },
                new DocTypeIndexDto { DocumentTypeId = 2, IndexId = 2, Required = false },
            };

            DocumentTypeGroupDto documentTypeGroupDto = new DocumentTypeGroupDto();
            IndexInfoDto ındexInfoDto = new IndexInfoDto();

            mockDocumentTypeGroupService.Setup(_ => _.GetByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeGroupDto);
            mockDocumentTypeService.Setup(_ => _.GetByGroupIdAsync(It.IsAny<long>())).ReturnsAsync(documentTypeDtos);
            

            string shortname = "test shortname";
            var result = await _controller.GetDocumentTypesByGroup(shortname, "", "");
            var actual = (result.Result as ObjectResult).Value as List<DocumentType>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.Empty);

        }

        [Test]
        public async Task To_Document_Type_List()
        {
            List<DocumentTypeDto> documentTypeDtoList = new List<DocumentTypeDto> {
                new DocumentTypeDto
                {
                    Id = 1,
                    Name = "Test"
                }
            };

            List<DocTypeIndexDto> docTypeIndexDtoList = new List<DocTypeIndexDto> {
                new DocTypeIndexDto
                {
                    DocumentTypeId = 1,
                    IndexId = 2
                }
            };

            IndexInfoDto indexInfoDto = new IndexInfoDto
            {
                Name = "index test"
            };

            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test" }
            };

            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtoList);
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(indexInfoDto);

            var result = await _controller.ToDocumentTypeList(documentTypeDtos);
            IEnumerable<IndexInfo> indexInfoList = result.First().IndexInfoList;
            Assert.Multiple(() =>
            {
                Assert.That(indexInfoList, Is.Not.Null);
                Assert.That(result, Is.Not.Null);
            });
            Assert.That(indexInfoList.First().Name, Is.EqualTo("index test"));
        }

        [Test]
        public async Task To_Document_Type_List_Continue()
        {
            DocumentTypeDto? dto = null;
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto> { dto };

            var result = await _controller.ToDocumentTypeList(documentTypeDtos);

            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task To_Document_Type_List_Continue_2()
        {
            DocumentTypeDto dto = new DocumentTypeDto() { Id = 3 };
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto> { dto };

            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto> { 
                new DocTypeIndexDto() { Id = 1 }
            };
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);

            IndexInfoDto? dto2 = null;
            mockDocumentIndexInfoService.Setup(_ => _.GetAsync(It.IsAny<long>())).ReturnsAsync(dto2);

            var result = await _controller.ToDocumentTypeList(documentTypeDtos);

            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result.First().Id, Is.EqualTo(3));
        }


        [Test]
        public async Task Get_Document_Types_By_Status()
        { 
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test1", DocumentTypeGroupId = 1, Status = Status.ACTIVE },
                new DocumentTypeDto { Id = 2, Name = "Test2", DocumentTypeGroupId = 2, Status = Status.PASSIVE },
                new DocumentTypeDto { Id = 3, Name = "Test3", DocumentTypeGroupId = 3, Status = Status.ACTIVE },
            };

            DocumentTypeGroupDto documentTypeGroupDto = new DocumentTypeGroupDto
            {
                Name = "test document type group name",
                Description = "test document type group description"
            };

            mockDocumentTypeService.Setup(_ => _.GetAllAsync(Status.ACTIVE)).ReturnsAsync(documentTypeDtos);
            mockDocumentTypeGroupService.Setup(_ => _.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(documentTypeGroupDto);

            int status = 1;
            var result = await _controller.GetDocumentTypesByStatus(status, "", "");
            var actual = (result.Result as ObjectResult).Value as List<ResponseDocumentTypes>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<List<ResponseDocumentTypes>>());
            Assert.That(actual, Has.Count.EqualTo(3));
            Assert.That(actual.First().Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task Get_Document_Types_By_Status_Not_Found()
        {
            List<DocumentTypeDto>? documentTypeDtos = null;

            mockDocumentTypeService.Setup(_ => _.GetAllAsync(Status.ACTIVE)).ReturnsAsync(documentTypeDtos);

            int status = 1;
            var result = await _controller.GetDocumentTypesByStatus(status, "", "");
           
            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());

        }

        [Test]
        public async Task Get_Document_Types_Not_Found()
        {
            List<DocumentTypeDto>? documentTypeDtos = null;

            mockDocumentTypeService.Setup(_ => _.GetAllAsync(Status.ACTIVE)).ReturnsAsync(documentTypeDtos);

            int status = 1;
            var result = await _controller.GetDocumentTypesByStatus(status, "", "");

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Get_Document_Types_By_Status_Continue()
        {
            DocumentTypeDto? dto = null;
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto> { dto };

            mockDocumentTypeService.Setup(_ => _.GetAllAsync(Status.ACTIVE)).ReturnsAsync(documentTypeDtos);

            int status = 1;
            var result = await _controller.GetDocumentTypesByStatus(status, "", "");
            var actual = (result.Result as ObjectResult).Value as List<ResponseDocumentTypes>;

            Assert.That(actual, Is.Empty);
        }

        [Test]
        public async Task Get_Document_Types_By_Name_And_Status_Not_Found()
        {
            List<DocumentTypeDto>? documentTypeDtos = null;

            mockDocumentTypeService.Setup(_ => _.SearchDocumentTypesByNameAndStatusAsync(It.IsAny<string>(), It.IsAny<Status>())).ReturnsAsync(documentTypeDtos);

            RequestSearchDocumentType dto = new RequestSearchDocumentType();
            var result = await _controller.GetDocumentTypesByNameAndStatus(dto, "", "");

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

        [Test]
        public async Task Get_Document_Types_By_Name_And_Status()
        { 
            List<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>
            {
                new DocumentTypeDto { Id = 1, Name = "Test1" },
                new DocumentTypeDto { Id = 2, Name = "Test2" },
            };

            RequestSearchDocumentType requestSearchDocumentType = new RequestSearchDocumentType
            {
                Name = "test request name",
                Status = Status.ACTIVE
            };

            mockDocumentTypeService.Setup(_ => _.SearchDocumentTypesByNameAndStatusAsync(It.IsAny<string>(), It.IsAny<Status>())).ReturnsAsync(documentTypeDtos);

            var result = await _controller.GetDocumentTypesByNameAndStatus(requestSearchDocumentType, "", "");
            var actual = (result.Result as ObjectResult).Value as List<DocumentType>;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<List<DocumentType>>());
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual.First().Name, Is.EqualTo("Test1"));
        }

        [Test]
        public async Task Create_Document_Type()
        {
            List<IndexModel> indexInfoList = new List<IndexModel> {
                new IndexModel { ShortName = "test index model short name 1", Required = true}             
            };

            RequestCreateDocumentType requestCreateDocumentType = new RequestCreateDocumentType
            {
                Name = "test name",
                ShortName = "test short name",
                IndexInfoList = indexInfoList
            };

            IndexInfoDto ındexInfoDto = new IndexInfoDto
            {
                Id = 1,
                Name = "test name",
                Description = "test index model short name 1"
            };
            DocumentTypeDto documentTypeDto = new DocumentTypeDto
            {
                Id = 1,
                Description = "test index model short name 1"
            };
            DocTypeIndexDto docTypeIndexDto = new DocTypeIndexDto
            {
                Id = 1,
                Required = true
            };

            mockDocumentTypeService.Setup(_ => _.CreateAsync(It.IsAny<DocumentTypeDto>())).ReturnsAsync(documentTypeDto);
            mockDocumentIndexInfoService.Setup(_ => _.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync(ındexInfoDto);
            mockDocTypeIndexService.Setup(_ => _.CreateAsync(It.IsAny<DocTypeIndexDto>())).ReturnsAsync(docTypeIndexDto);

            var result = await _controller.CreateDocumentType(requestCreateDocumentType, "", "");
            var actual = (result.Result as ObjectResult).Value as DocumentType;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<DocumentType>());
            Assert.That(actual.IndexInfoList.First().ShortName, Is.EqualTo("test index model short name 1"));
        }

        [Test]
        public async Task Update_Document_Type()
        {
            List<IndexModel> indexInfoList = new List<IndexModel> {
                new IndexModel { ShortName = "test index model short name 1", Required = true}
            };

            RequestUpdateDocumentType requestUpdateDocumentType = new RequestUpdateDocumentType
            {
                Name = "test name",
                ShortName = "test short name",
                IndexInfoList = indexInfoList
            };

            IndexInfoDto ındexInfoDto = new IndexInfoDto
            {
                Id = 1,
                Name = "test name",
                Description = "test index model short name 1"
            };
            DocumentTypeDto documentTypeDto = new DocumentTypeDto
            {
                Id = 1,
                Description = "test index model short name 1"
            };
            DocTypeIndexDto docTypeIndexDto = new DocTypeIndexDto
            {
                Id = 1,
                Required = true
            };
            List<DocTypeIndexDto> docTypeIndexDtos = new List<DocTypeIndexDto>
            {
                docTypeIndexDto
            };

            int deletedFlag = 1;
            mockDocumentTypeService.Setup(_ => _.UpdateAsync(It.IsAny<DocumentTypeDto>())).ReturnsAsync(documentTypeDto);
            mockDocTypeIndexService.Setup(_ => _.GetByDocumentTypeAsync(It.IsAny<long>())).ReturnsAsync(docTypeIndexDtos);
            mockDocTypeIndexService.Setup(_ => _.DeleteAsync(It.IsAny<DocTypeIndexDto>())).ReturnsAsync(deletedFlag);
            mockDocumentIndexInfoService.Setup(_ => _.GetByDescriptionAsync(It.IsAny<string>())).ReturnsAsync(ındexInfoDto);
            mockDocTypeIndexService.Setup(_ => _.CreateIfNotExistsAsync(It.IsAny<DocTypeIndexDto>())).ReturnsAsync(docTypeIndexDto);

            var result = await _controller.UpdateDocumentType(requestUpdateDocumentType, "", "");
            var actual = (result.Result as ObjectResult).Value as DocumentType;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<DocumentType>());
            Assert.That(actual.IndexInfoList.First().ShortName, Is.EqualTo("test index model short name 1"));
        }

        [Test]
        public async Task Delete_Document_Type()
        {          
            DocumentTypeDto documentTypeDto = new DocumentTypeDto
            {
                Id = 1,
                Description = "test description"
            };
        
            mockDocumentTypeService.Setup(_ => _.SoftDeleteAsync(It.IsAny<long>())).ReturnsAsync(documentTypeDto);

            int deletedFlag = 1;
            var result = await _controller.DeleteDocumentType(deletedFlag, "", "");
            var actual = (result as ObjectResult).Value as DocumentType;

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.InstanceOf<DocumentType>());
            Assert.That(actual.ShortName, Is.EqualTo("test description"));
        }

        // Test for Not Found Result
        [Test]
        public async Task Document_Type_Not_Found_Result_Test()
        {
            DocumentTypeDto? documentTypeDto = null;
            mockDocumentTypeService.Setup(_ => _.GetActiveByShortNameAsync(It.IsAny<string>())).ReturnsAsync(documentTypeDto);

            string shortName = "test shortname";
            var result = await _controller.GetDocumentType(shortName, "", "");

            Assert.That(result.Result, Is.InstanceOf<NotFoundResult>());
        }

    }
}
