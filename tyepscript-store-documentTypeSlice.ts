import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "./../index";
import {
  DocumentType,
  DocumentTypeDto,
  IndexInfo,
  IndexInfoItemDto,
  Role,
  RoleItemDto,
  RuleResponse,
  SbuType,
} from "src/components/GetDocument/types";

export type DocumentTypeState = {
  documentTypesLogs: DocumentType[];
  filter: any;
  pagination: any;
  branchData: unknown[];
  pageNum: number;
  selectedRows: DocumentType[];
  editFlag: number;
  selectedDocumentType: DocumentType | null;
  actionPending: boolean;
  documentTypesLoading: boolean;
  indexInfos: IndexInfo[];
  indexInfosLoading: boolean;
  filterName: string;
  filterShortName: string;
  formFields: DocumentTypeDto;
  indexInfoArray: IndexInfoItemDto[];
  roleArray: RoleItemDto[];
  selectedRow: number[];
  indexOperationNotDone: boolean;
  roles: Role[];
  roleLoading: boolean;
  documentTypeRules: RuleResponse[];
  documentTypeIdForTypeRule: number | undefined;
  roleOperationNotDone: boolean;
  sbuTypes: SbuType[];
};

const initialState: DocumentTypeState = {
  documentTypesLogs: [],
  filter: {},
  pagination: { current: 1, pageSize: 20 },
  branchData: [],
  pageNum: 0,
  selectedRows: [],
  editFlag: 0,
  selectedDocumentType: null,
  actionPending: false,
  documentTypesLoading: false,
  indexInfos: [],
  indexInfosLoading: false,
  filterName: "",
  filterShortName: "",
  formFields: { id: undefined, name: "", shortName: "", status: "1", documentTypeGroupId: 1, indexInfoList: [] },
  indexInfoArray: [],
  selectedRow: [],
  indexOperationNotDone: false,
  roles: [],
  roleLoading: false,
  roleArray: [],
  documentTypeRules: [],
  documentTypeIdForTypeRule: 1,
  roleOperationNotDone: false,
  sbuTypes: [],
};

export const documentTypeSlice = createSlice({
  name: "documentTypeData",
  initialState,
  reducers: {
    setDocumentTypesLogs: (state: DocumentTypeState, action: PayloadAction<DocumentType[]>) => {
      state.documentTypesLogs = action.payload;
    },
    setPagination: (state: DocumentTypeState, action: PayloadAction<any>) => {
      state.pagination = action.payload;
    },
    setFilter: (state: DocumentTypeState, action: PayloadAction<any>) => {
      state.filter = action.payload;
    },
    setBranchData: (state: DocumentTypeState, action: PayloadAction<unknown[]>) => {
      state.branchData = action.payload;
    },
    setPageNum: (state: DocumentTypeState, action: PayloadAction<number>) => {
      state.pageNum = action.payload;
    },
    setSelectedRows: (state: DocumentTypeState, action: PayloadAction<DocumentType[]>) => {
      state.selectedRows = action.payload;
    },
    setEditFlag: (state: DocumentTypeState, action: PayloadAction<number>) => {
      state.editFlag = action.payload;
    },
    setSelectedDocumentType: (state: DocumentTypeState, action: PayloadAction<DocumentType | null>) => {
      state.selectedDocumentType = action.payload;
    },
    setDocumentTypeActionPending: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.actionPending = action.payload;
    },
    setDocumentTypesLoading: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.documentTypesLoading = action.payload;
    },
    setIndexInfos: (state: DocumentTypeState, action: PayloadAction<IndexInfo[]>) => {
      state.indexInfos = action.payload;
    },
    setIndexInfosLoading: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.indexInfosLoading = action.payload;
    },
    setFilterName: (state: DocumentTypeState, action: PayloadAction<string>) => {
      state.filterName = action.payload;
    },
    setFilterShortName: (state: DocumentTypeState, action: PayloadAction<string>) => {
      state.filterShortName = action.payload;
    },
    setFormFields: (state: DocumentTypeState, action: PayloadAction<DocumentTypeDto>) => {
      state.formFields = action.payload;
    },
    setIndexInfoArray: (state: DocumentTypeState, action: PayloadAction<IndexInfoItemDto[]>) => {
      state.indexInfoArray = action.payload;
    },
    setSelectedRow: (state: DocumentTypeState, action: PayloadAction<number[]>) => {
      state.selectedRow = action.payload;
    },
    setIndexOperationNotDone: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.indexOperationNotDone = action.payload;
    },
    setRoleOperationNotDone: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.roleOperationNotDone = action.payload;
    },
    setRoles: (state: DocumentTypeState, action: PayloadAction<Role[]>) => {
      state.roles = action.payload;
    },
    setRolesLoading: (state: DocumentTypeState, action: PayloadAction<boolean>) => {
      state.roleLoading = action.payload;
    },
    setRoleArray: (state: DocumentTypeState, action: PayloadAction<RoleItemDto[]>) => {
      state.roleArray = action.payload;
    },
    setDocumentTypeRules: (state: DocumentTypeState, action: PayloadAction<RuleResponse[]>) => {
      state.documentTypeRules = action.payload;
    },
    setDocumentTypeIdForTypeRule: (state: DocumentTypeState, action: PayloadAction<number>) => {
      state.documentTypeIdForTypeRule = action.payload;
    },
    setSbuTypes: (state: DocumentTypeState, action: PayloadAction<SbuType[]>) => {
      state.sbuTypes = action.payload;
    },
  },
});

export const {
  setDocumentTypesLogs,
  setPagination,
  setFilter,
  setBranchData,
  setPageNum,
  setSelectedRows,
  setEditFlag,
  setSelectedDocumentType,
  setDocumentTypeActionPending,
  setDocumentTypesLoading,
  setIndexInfos,
  setIndexInfosLoading,
  setFilterName,
  setFilterShortName,
  setFormFields,
  setIndexInfoArray,
  setSelectedRow,
  setIndexOperationNotDone,
  setRoleOperationNotDone,
  setRoles,
  setRolesLoading,
  setRoleArray,
  setDocumentTypeRules,
  setDocumentTypeIdForTypeRule,
  setSbuTypes,
} = documentTypeSlice.actions;
export const documentTypeStore = (state: RootState) => state.documentTypeReducer;
export default documentTypeSlice.reducer;
