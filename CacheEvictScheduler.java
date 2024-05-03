package com.ykb.cosmos.dms.batch;

import com.ykb.cosmos.dms.external.edison.client.EdisonApi;
import com.ykb.cosmos.dms.util.ExceptionUtil;
import lombok.RequiredArgsConstructor;
import lombok.extern.slf4j.Slf4j;
import org.springframework.cache.CacheManager;
import org.springframework.http.ResponseEntity;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Component;

import java.util.Objects;
import java.util.stream.Collectors;

@Slf4j
@Component
@RequiredArgsConstructor
public class CacheEvictScheduler {

    private final CacheManager cacheManager;
    private final EdisonApi edisonApi;

    @Scheduled(cron = "0 0 0 * * *")
    public void resetCaches() {
        log.info("resetCaches clearing caches {{}}.", cacheManager.getCacheNames().stream().collect(Collectors.joining(",")));
        cacheManager.getCacheNames()
                .parallelStream()
                .filter(Objects::nonNull)
                .forEach(cacheName -> Objects.requireNonNull(cacheManager.getCache(cacheName)).clear());

        clearEdisonCache();
    }

    private void clearEdisonCache() {
        try {
            ResponseEntity<Object> responseEntity = edisonApi.clearAllCaches();
            log.info("Clearing edison Caches {}", responseEntity.getStatusCode());
        } catch (Exception e) {
            log.error("Clearing edison Caches {}", ExceptionUtil.convertStackTraceToString(e));
        }
    }

    @Scheduled(cron = "@hourly") // run for every one hour
    public void resetDocumentTypeRuleCache() {
        log.info("clearing DocumentTypeRule getPermission cache {}", cacheManager.getCache("getPermission"));
        Objects.requireNonNull(cacheManager.getCache("getPermission")).clear();
    }

}
