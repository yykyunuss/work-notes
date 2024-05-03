using AutoMapper;
using Cosmos.Edison.NetCore.Business.Services;
using Cosmos.Edison.NetCore.Business.Caches;
using Moq;
using Cosmos.Edison.NetCore.Business.Dto.Document;
using System.Linq.Expressions;
using Cosmos.Edison.NetCore.Business.Enums;
using Cosmos.Edison.NetCore.Data.Poco.Document;
using Cosmos.Edison.NetCore.Data.Repositories;
using Cosmos.Edison.NetCore.Business.Settings;

namespace Cosmos.Edison.NetCore.Tests.ServiceTests
{
    // Service To Be Test
    public class DocumentTypeServiceTests
    {
        private DocumentTypeService documentTypeService;

        private static readonly Mock<DocumentTypeCache> mockTemplateCache = new();
        private static readonly Mock<DocumentStorageAreaCache> mockDocStorageAreaCache = new();
        
        // Mapper
        private static readonly MapperConfiguration mappingConfig = new(mc =>
        {
            mc.AddProfile(new DocumentTypeDtoProfile());
        });
        private static readonly IMapper mapper = mappingConfig.CreateMapper();

        private Mock<DocumentTypeRepository> mockDocumentTypeRepository;

        private DocumentTypeSetting documentTypeSetting;
        public DocumentTypeServiceTests()
        {
            documentTypeSetting = new DocumentTypeSetting() { RelativePath = "relativepath"};
            mockDocumentTypeRepository = new Mock<DocumentTypeRepository>();
            documentTypeService ??= new DocumentTypeService(mockTemplateCache.Object, mockDocumentTypeRepository.Object, mockDocStorageAreaCache.Object, mapper, documentTypeSetting);
        }


        [Test]
        public async Task Test_Get_All_Document_Types_Async()
        {
            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();
            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> result = await documentTypeService.GetAllAsync();
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
            Assert.That(result, Is.EqualTo(documentTypeDtos));
        }


        [Test]
        public async Task Given_Default_Param_Returns_DocumentTypeDto()
        {
            string shortName = "testName";
            DocumentTypeDto documentTypeDto = new()
            {
                Name = "Test",
                Description = shortName
            };
            mockTemplateCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDto);
            DocumentTypeDto activeDocumentTypeDto = await documentTypeService.GetActiveByShortNameAsync(shortName);
            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(shortName, Is.EqualTo(activeDocumentTypeDto.Description));
        }

        [Test]
        public async Task Get_Document_Types_By_ShortName_Async()
        {
            string shortName = "testName";

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.GetDocumentTypesByShortNameAsync(shortName);
            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Get_Document_Types_By_ShortName_List_Async()
        {
            List<string> shortNames = new() { "testName1", "testName2" };

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.GetDocumentTypesByShortNameListAsync(shortNames);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Get_Document_Types_By_ShortName_List_Async_Null_Case()
        {
            List<string> shortNames = new();

            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.GetDocumentTypesByShortNameListAsync(shortNames);

            Assert.That(activeDocumentTypeDto, Is.Null);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_And_Status_Async_1()
        {
            string name = "test name";
            Status status = Status.ACTIVE;
            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAndStatusAsync(name, status);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_And_Status_Async_Where_Row()
        {
            string name = "test name";
            Status status = Status.ACTIVE;
            IEnumerable<DocumentTypeDto> documentTypeDtos = new List<DocumentTypeDto>();
            {
                _ = new DocumentTypeDto { Id = 1, Name = "test name", Status = Status.ACTIVE };
            };

            DocumentTypeDto dto = new DocumentTypeDto { Id = 1, Name = "test name", Status = Status.ACTIVE };
            mockTemplateCache.Setup(x => x.ListAsync(dto => dto.Status == Status.ACTIVE)).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAndStatusAsync(name, status);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_And_Status_Async_2()
        {
            string name = "test name";
            
            IEnumerable<PocoDocumentType> documentTypeDtos = Enumerable.Empty<PocoDocumentType>();

            mockDocumentTypeRepository.Setup(x => x.FetchAsync(It.IsAny<string>(), It.IsAny<object[]>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAndStatusAsync(name, null);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_And_Status_Async_3()
        {
            string name = " ";
            Status status = Status.ACTIVE;

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAndStatusAsync(name, status);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_And_Status_Async_4()
        {
            string name = " ";

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAndStatusAsync(name, null);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_Document_Types_By_Name_Async_Null_Case()
        {
            string name = "";

            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAsync(name);

            Assert.That(activeDocumentTypeDto, Is.Null);
        }
        [Test]
        public async Task Search_DocumentTypes_By_Name_Async_1()
        {
            string name = "test";
            bool isExactMatch = true;
            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAsync(name, isExactMatch);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_Async_2()
        {
            string name = "test";
            bool isExactMatch = false;
            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameAsync(name, isExactMatch);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_By_Status_Async_1()
        {
            string name = "test";
            Status status = Status.ACTIVE;

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameByStatusAsync(name, status);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto, Is.Empty);
        }

        [Test]
        public async Task Search_DocumentTypes_By_Name_By_Status_Async_2()
        {
            string name = " ";
            Status status = Status.ACTIVE;

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDto = await documentTypeService.SearchDocumentTypesByNameByStatusAsync(name, status);

            Assert.That(activeDocumentTypeDto, Is.Null);
        }

        [Test]
        public async Task Get_By_Id_Async()
        {
            long id = 2;

            DocumentTypeDto documentTypeDto = new DocumentTypeDto { Name = "test1", Description = "Description1" };

            mockTemplateCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDto);
            DocumentTypeDto activeDocumentTypeDto = await documentTypeService.GetByIdAsync(id); ;

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto.Name, Is.EqualTo("test1"));
        }

        [Test]
        public async Task Get_By_Group_Id_Async()
        {
            long id = 2;

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDtos = await documentTypeService.GetByGroupIdAsync(id); ;

            Assert.That(activeDocumentTypeDtos, Is.Not.Null);
            Assert.That(activeDocumentTypeDtos, Is.Empty);
        }

        [Test]
        public async Task Get_By_Group_Id_List_Async_1()
        {
            List<long> ids = new() { 1, 2, 3};

            IEnumerable<DocumentTypeDto> documentTypeDtos = Enumerable.Empty<DocumentTypeDto>();

            mockTemplateCache.Setup(x => x.ListAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDtos);
            IEnumerable<DocumentTypeDto> activeDocumentTypeDtos = await documentTypeService.GetByGroupIdListAsync(ids);

            Assert.That(activeDocumentTypeDtos, Is.Not.Null);
            Assert.That(activeDocumentTypeDtos, Is.Empty);
        }

        [Test]
        public async Task Get_By_Group_Id_List_Async_2()
        {
            List<long> ids = new();

            IEnumerable<DocumentTypeDto> activeDocumentTypeDtos = await documentTypeService.GetByGroupIdListAsync(ids);

            Assert.That(activeDocumentTypeDtos, Is.Null);
        }

        [Test]
        public async Task Create_Async()
        {
            IEnumerable<PocoDocumentType> documentTypeDtos = new List<PocoDocumentType> { new PocoDocumentType { Name = "test name 1", Description = "test description 1" } };
            PocoDocumentType pocoDocumentType = new() { Name = "test name 2", Description = "test description 2" };

            mockDocumentTypeRepository.Setup(x => x.QueryAsync(It.IsAny<Expression<Func<PocoDocumentType, bool>>>())).ReturnsAsync(documentTypeDtos);
            int returnValue = 1;
            mockDocumentTypeRepository.Setup(x => x.UpdateAsync(It.IsAny<PocoDocumentType>())).ReturnsAsync(returnValue);

            DocumentStorageAreaDto? documentStorageAreaDto = null;
            mockDocStorageAreaCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentStorageAreaDto, bool>>>())).ReturnsAsync(documentStorageAreaDto);

            DocumentTypeDto documentTypeDto = new() { Id = 1 };
            DocumentTypeDto activeDocumentTypeDto = await documentTypeService.CreateAsync(documentTypeDto);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto.Id, Is.EqualTo(1));
        }

        [Test]
        public async Task Create_Async_StorageAreaDto_Null_Case()
        {
            IEnumerable<PocoDocumentType> documentTypeDtos = new List<PocoDocumentType> { new PocoDocumentType { Name = "test name 1", Description = "test description 1" } };
            PocoDocumentType pocoDocumentType = new() { Name = "test name 2", Description = "test description 2" };

            mockDocumentTypeRepository.Setup(x => x.QueryAsync(It.IsAny<Expression<Func<PocoDocumentType, bool>>>())).ReturnsAsync(documentTypeDtos);

            int returnValue1 = 1;
            mockDocumentTypeRepository.Setup(x => x.UpdateAsync(It.IsAny<PocoDocumentType>())).ReturnsAsync(returnValue1);

            DocumentStorageAreaDto documentStorageAreaDto = new DocumentStorageAreaDto() { Id = 1 };
            mockDocStorageAreaCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentStorageAreaDto, bool>>>())).ReturnsAsync(documentStorageAreaDto);

            long returnValue = 2;
            mockDocumentTypeRepository.Setup(x => x.InsertAsync<long>(It.IsAny<PocoDocumentType>())).ReturnsAsync(returnValue);

            mockTemplateCache.Setup(x => x.InvalidateCache(false));

            DocumentTypeDto documentTypeDto = new() { Id = 1, Description = "description", Status = Status.ACTIVE };
            DocumentTypeDto activeDocumentTypeDto = await documentTypeService.CreateAsync(documentTypeDto);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto.Id, Is.EqualTo(2));
        }

        // Test for exception
        [Test]
        public void Create_Async_No_Difference_Exception()
        {
            IEnumerable<PocoDocumentType> documentTypeDtos = new List<PocoDocumentType> { new PocoDocumentType { Id = 2, Name = "test name 2", Description = "test description 2", Status = (short)Status.ACTIVE } };
            DocumentTypeDto documentTypeDto = new DocumentTypeDto { Id = 2, Name = "test name 2", Description = "test description 2", Status = Status.ACTIVE };

            mockDocumentTypeRepository.Setup(x => x.QueryAsync(It.IsAny<Expression<Func<PocoDocumentType, bool>>>())).ReturnsAsync(documentTypeDtos);

            Func<Task> testDelegate = async () => await documentTypeService.CreateAsync(documentTypeDto);

            Assert.That(testDelegate, Throws.Exception);
        }

        [Test]
        public async Task Update_Async()
        {            
            PocoDocumentType pocoDocumentType = new PocoDocumentType();
            mockDocumentTypeRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(pocoDocumentType);

            DocumentTypeDto documentTypeDto = new DocumentTypeDto { Id = 2, Name = "test name 2", Description = "test description 2", Status = Status.ACTIVE };
            mockTemplateCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDto);

            int returnValue = 1;
            mockDocumentTypeRepository.Setup(x => x.UpdateAsync(It.IsAny<PocoDocumentType>())).ReturnsAsync(returnValue);

            mockTemplateCache.Setup(x => x.InvalidateCache(false));

            DocumentTypeDto dto = new DocumentTypeDto { Id = 2 };
            var result = await documentTypeService.UpdateAsync(dto);

            Assert.That(result, Is.Not.Null);
        }

        // Test for exception
        [Test]
        public void Update_Async_OldPoco_Null_Exception()
        {
            PocoDocumentType? pocoDocumentType = null;
            mockDocumentTypeRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(pocoDocumentType);

            DocumentTypeDto? documentTypeDto = null;
            mockTemplateCache.Setup(x => x.SingleOrDefaultAsync(It.IsAny<Expression<Func<DocumentTypeDto, bool>>>())).ReturnsAsync(documentTypeDto);

            DocumentTypeDto dto = new DocumentTypeDto { Id = 1, Name = "test name 2", Description = "test description 2" };

            Func<Task> testDelegate = async () => await documentTypeService.UpdateAsync(dto);
            Assert.That(testDelegate, Throws.Exception);
        }

        [Test]
        public async Task Soft_Delete_Async()
        {
            PocoDocumentType pocoDocumentType = new PocoDocumentType { Name = "test name 2", Description = "test description 2" };
            mockDocumentTypeRepository.Setup(x => x.GetByIdAsync(It.IsAny<long>())).ReturnsAsync(pocoDocumentType);

            int returnValue = 1;
            mockDocumentTypeRepository.Setup(x => x.UpdateAsync(It.IsAny<PocoDocumentType>())).ReturnsAsync(returnValue);
            
            mockTemplateCache.Setup(x => x.InvalidateCache(false));

            long id = 1;
            DocumentTypeDto activeDocumentTypeDto = await documentTypeService.SoftDeleteAsync(id);

            Assert.That(activeDocumentTypeDto, Is.Not.Null);
            Assert.That(activeDocumentTypeDto.Name, Is.EqualTo("test name 2"));
        }
    }
}
