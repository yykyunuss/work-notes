package com.ykb.cosmos.dms.web.advice;

import com.ykb.cosmos.dms.exception.DmsException;
import com.ykb.nl.microcommonutilstarter.dto.BaseResponse;
import com.ykb.nl.microcommonutilstarter.util.ExceptionUtil;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.core.Ordered;
import org.springframework.core.annotation.Order;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;

import javax.annotation.PostConstruct;

@Order(3) // Highest Precedence olarak degisecek
@RestControllerAdvice
public class DmsExceptionHandler {

    private static final Logger log = LoggerFactory.getLogger(DmsExceptionHandler.class);

    public DmsExceptionHandler() {
    }

    @PostConstruct
    void init() {
        log.info("DmsExceptionHandler init on");
    }

    @ExceptionHandler({DmsException.class})
    protected ResponseEntity<BaseResponse<Object>> handleException(DmsException ex) {
        String stackTraceToString = ExceptionUtil.convertStackTraceToString(ex);
        log.error(stackTraceToString);
        return (new BaseResponse(HttpStatus.OK)).fail(ex.getMessage());
    }
}
