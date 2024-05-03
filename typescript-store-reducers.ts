import globalReducer from "./GlobalSlice";
import documentInquiryReducer from "./DocumentInquirySlice";
import documentTemplateReducer from "./document-template/slice";
import documentTypeReducer from "./document-type-management/documentTypeSlice";
import documentTemplateEntityReducer from "./DocumentTemplateEntitySlice";
import documentUploadReducer from "./document-upload/documentUploadSlice";

export default {
  globalReducer,
  documentUploadReducer,
  documentTemplateReducer,
  documentInquiryReducer,
  documentTypeReducer,
  documentTemplateEntityReducer,
};
