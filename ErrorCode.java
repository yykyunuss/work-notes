package com.ykb.cosmos.dms.exception;

import com.ykb.nl.microcommonutilstarter.exception.ExceptionConstants.ErrorCodeMessagePair;

public enum ErrorCode {

	DRAFT_NOT_FOUND("E005", "Draft not found."),
	CANNOT_CONVERT_DOCUMENT("E006", "Error occurred while converting document."),
	TEMPLATE_NOT_FOUND("E007", "Template not found. template id: %s"),
	DOCUMENT_CANNOT_CREATED("E008", "Error occurred while creating document."),
	TEMPLATE_CONTENT_NOT_FOUND("E009", "Template content not found. template id: %s"),
	CANNOT_SERIALIZE_JSON_DATA("E010", "Data input cannot be serialized."),
	INVALID_DRAFT_UNIQUE_KEY("E011", "Invalid draft Unique key: %s"),
	RULE_NOT_FOUND("E012", "Rule not found. ruleId: %s"),
	EXPRESSION_COULD_NOT_PARSE("E013", "Rule could not parse. Error: %s"),
	EXPRESSION_COULD_NOT_EVALUATE("E014", "Rule could not evaluate. Error: %s"),
	DOCUMENT_PROCESS_DEFINITION_NOT_FOUND("E015", "Document Process Definition not found. Error: %s"),
	DOCUMENT_PROCESS_DEFINITION_COULD_NOT_UPDATED("E016", "Document Process Definition could not updated. Error: %s"),
	PASSIVE_DOCUMENT_COULD_NOT_INDEX("E017", "Passive Document Couldn't index."),
	INDEXED_DOCUMENT_COULD_NOT_INDEX("E018", "Indexed Document Couldn't index."),
	PENDING_DOCUMENT_COULD_NOT_INDEX("E019", "Pending Document Couldn't index."),
	PENDING_DOCUMENT_COULD_NOT_DELETE("E020", "Pending Document Couldn't delete."),
	DOCUMENT_TYPE_RULE_NOT_FOUND("E021", "Document Type Rule not found. Error: %s");


	private final String errorCode;
	private final String errorMessage;

	ErrorCode(String errorCode, String errorMessage) {
		this.errorCode = errorCode;
		this.errorMessage = errorMessage;
	}

	public ErrorCodeMessagePair getErrorMessagePair() {
		return ErrorCodeMessagePair.createNew(this.errorCode, this.errorMessage);
	}
	
	public ErrorCodeMessagePair getErrorMessagePair(Object... params) {
		return ErrorCodeMessagePair.createNew(this.errorCode, String.format(this.errorMessage, params));
	}

}
