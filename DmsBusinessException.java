package com.ykb.cosmos.dms.exception;

import com.ykb.nl.microcommonutilstarter.exception.BaseException;

public class DmsBusinessException extends BaseException {


	public DmsBusinessException(ErrorCode errorCode) {
		super(errorCode.getErrorMessagePair());
    }

	public DmsBusinessException(ErrorCode errorCode, Object... params) {
		super(errorCode.getErrorMessagePair(params));
	}

}
