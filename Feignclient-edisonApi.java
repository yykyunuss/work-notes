package com.ykb.cosmos.dms.external.edison.client;

import com.ykb.cosmos.dms.dto.*;
import com.ykb.cosmos.dms.external.edison.dto.DocumentTypeRequestDto;
import com.ykb.cosmos.dms.external.edison.dto.IndexInfo;
import com.ykb.cosmos.dms.external.edison.dto.ResponseDocumentTypeDto;
import com.ykb.cosmos.dms.external.workflow.config.BaseFeignClientConfiguration;
import org.springframework.cloud.openfeign.FeignClient;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.util.List;

// configuration => EdisonErrorDecoder olarak degisecek
/*
config:

  external:
    edison:
      name: EdisonApi
      url: https://be-tst.factoring.yapikredi.com.tr/fctr-edison-api/api

*/
@FeignClient(name = "EdisonApi", url = "${external.edison.url}", configuration = BaseFeignClientConfiguration.class, fallback = EdisonApiFallback.class)
public interface EdisonApi {

    @GetMapping(path = "/documentTypes")
    ResponseEntity<List<ResponseDocumentTypeDto>> getDocumentTypes();

    @GetMapping(path = "/documentTypes")
    ResponseEntity<List<ResponseDocumentTypeDto>> getDocumentTypeById(@RequestParam Long id);

    @PostMapping(path = "/Documents/search")
    ResponseEntity<ResponseGetDocumentPage> getDocuments(@RequestBody RequestSearchDocuments requestDto);

    @PostMapping("/documentTypes/search")
    ResponseEntity<List<ResponseDocumentTypeDto>> getDocumentTypesByNameAndStatus(@RequestBody RequestSearchDocumentTypes requestSearchDocumentTypes);

    @PostMapping("/documentTypes")
    ResponseEntity<DocumentTypeRequestDto> createDocumentType(@RequestBody DocumentTypeRequestDto dto);

    @PutMapping("/documentTypes")
    ResponseEntity<DocumentTypeRequestDto> updateDocumentType(@RequestBody DocumentTypeRequestDto dto);

    @DeleteMapping("/documentTypes/{id}")
    ResponseEntity<ResponseDocumentTypeDto> deleteDocumentType(@PathVariable(value = "id") String id);

    @GetMapping("/documentTypes/{shortName}")
    ResponseEntity<ResponseDocumentTypeDto> getDocumentTypeByShortName(@PathVariable(value = "shortName") String shortName);

    @GetMapping(path = "/Documents/download/{id}", consumes = {"text/plain"})
    ResponseEntity<byte[]> download(@PathVariable Long id);

    @GetMapping(path = "/Documents/{id}")
    ResponseEntity<DocumentItemDto> getDocumentById(@PathVariable(value = "id") String id);

    @PostMapping(path = "/Documents")
    ResponseEntity<DocumentItemDto> createDocument(@RequestBody RequestCreateDocument requestCreateDocument);

    @PutMapping(path = "/Documents")
    ResponseEntity<DocumentItemDto> updateDocument(@RequestBody RequestUpdateDocument requestUpdateDocument);

    @GetMapping(path = "/indexInfo")
    ResponseEntity<List<IndexInfo>> getIndexInfos();

    @GetMapping(path = "/documentTypes/status/{status}")
    ResponseEntity<List<ResponseDocumentTypeWithStatus>> getDocumentTypesWithStatus(@PathVariable(value = "status") String status);

    @PostMapping(path = "/Caches/all")
    ResponseEntity<Object> clearAllCaches();

}
