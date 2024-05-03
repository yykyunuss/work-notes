package com.ykb.cosmos.dms.util;

import com.ykb.cosmos.dms.dto.DobUploadRequestDto;
import com.ykb.cosmos.dms.support.email.graph.GraphMessageContext;
import com.ykb.nl.microcommonutilstarter.util.StringUtil;
import org.springframework.http.HttpHeaders;
import org.springframework.http.ResponseEntity;
import org.springframework.util.Assert;
import org.springframework.web.multipart.MultipartFile;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.nio.file.Files;
import java.time.OffsetDateTime;
import java.util.Base64;
import java.util.HashMap;
import java.util.Map;
import java.util.Objects;

public class CommonDocumentUtil {
    private static int maxDocumentSize = 10 * 1024 * 1024;

    private CommonDocumentUtil() {
    }

    public static void main(String[] args) {
        System.out.println(encodeFileToBase64("src\\main\\resources\\sample.pdf"));
    }

    private static String encodeFileToBase64(String filePath) {
        try {
            File file = new File(filePath);
            byte[] fileContent = Files.readAllBytes(file.toPath());
            return Base64.getEncoder().encodeToString(fileContent);
        } catch (IOException e) {
            throw new IllegalStateException("could not read file " + filePath, e);
        }
    }

    public static String getPrettyDocumentSize(byte[] imageByte) {
        String[] sizeNames = new String[]{"", "K", "M", "G", "T", "P", "E"};
        long bytes = imageByte.length;
        for (int i = 6; i > 0; i--) {
            double step = Math.pow(1024, i);
            if (bytes > step) return String.format("%3.1f %sB", bytes / step, sizeNames[i]).replace(",", ".");
        }
        return Long.toString(bytes);
    }

    public static void checkMaxDocumentSize(byte[] imageByte) {
        Assert.isTrue(imageByte == null || imageByte.length <= maxDocumentSize, "Maximum document size is 10MB");
    }

    public static String setContainerName(Long kifId, Long applicationNo) {
        String containerName;
        containerName = "c-" + applicationNo.toString();
        if (!Objects.isNull(kifId)) {
            containerName = "k-" + kifId.toString();
        }
        return containerName;

    }

    public static Map<String, String> setMetadataInfo(DobUploadRequestDto requestDto) {
        Map<String, String> metadata = new HashMap<>();
        metadata.put("applicationNo", requestDto.getApplicationNo().toString());
        metadata.put("documentTypeId", requestDto.getDocumentTypeId().toString());
        metadata.put("documentName", requestDto.getDocumentName());
        if (!Objects.isNull(requestDto.getKifId())) {
            metadata.put("kifId", requestDto.getKifId().toString());
        }
        metadata.put("openUser", requestDto.getOpenUser());
        metadata.put("UploadDate", OffsetDateTime.now().toString());
        metadata.put("Status", "A");
        metadata.put("ContainerName", setContainerName(requestDto.getKifId(), requestDto.getApplicationNo()));
        return metadata;
    }

    public static String getFilenameFromResponse(ResponseEntity<?> response) {
        HttpHeaders headers = response.getHeaders();
        String contentDisposition = headers.getFirst(HttpHeaders.CONTENT_DISPOSITION);
        String[] parts = contentDisposition.split(";");

        for (String part : parts) {
            if (part.trim().startsWith("filename=")) {
                String[] filenameParts = part.split("=");
                if (filenameParts.length == 2) {
                    String filename = filenameParts[1].trim();
                    if (filename.startsWith("\"") && filename.endsWith("\"")) {
                        // Remove double quotes if present
                        filename = filename.substring(1, filename.length() - 1);
                    }
                    return filename;
                }
            }
        }

        return null;
    }

    public static MultipartFile getMultipartFile(String documentName, String contentType, String content) {
        MultipartFile file = new MultipartFile() {
            @Override
            public String getName() {
                return documentName;
            }

            @Override
            public String getOriginalFilename() {
                return documentName;
            }

            @Override
            public String getContentType() {
                return contentType;
            }

            @Override
            public boolean isEmpty() {
                return StringUtil.isNullOrEmpty(content);
            }

            @Override
            public long getSize() {
                return content == null ? 0 : content.length();
            }

            @Override
            public byte[] getBytes() throws IOException {
                return content == null ? new byte[0] : Base64.getDecoder().decode(content.getBytes());
            }

            @Override
            public InputStream getInputStream() throws IOException {
                return new ByteArrayInputStream(Base64.getDecoder().decode(content.getBytes()));
            }

            @Override
            public void transferTo(File dest) throws IOException, IllegalStateException {

            }
        };
        return file;
    }

    public static MultipartFile getMultipartFile(GraphMessageContext.Attachment attachment) {
        return getMultipartFile(attachment.getName(), attachment.getContentType(), attachment.getContentBytes());
    }
}
