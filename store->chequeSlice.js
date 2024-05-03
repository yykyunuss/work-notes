import api from "../components/ChequeQuery/api";

const { createSlice } = require("@reduxjs/toolkit");

const initialState = {
  customerList: [],
  drawerList: [],
  intermediateEndorserList: [],
  lastEndorserList: [],
  chequeInfos: [],
  chequeNos: [],
  tableLoading: false,
  editCheque: {},
  updatedCheque: {},
  updatedCheques: [],
  deletedCheques: [],
  chequeId: 0,
  currencies: [],
  currenciesLoading: false,
  officialRates: [],
  officialRatesLoading: false,
  banks: [],
  banksLoading: false,
  branches: [],
};

export const getCurrenciesRequest = () => (dispatch, getState) => {
  dispatch(setCurrenciesLoading(true));

  api
    .get("/currency")
    .then(({ data }) => {
      dispatch(setCurrencies(data));
    })
    .catch((err) => {
      dispatch(setCurrencies([]));
    })
    .finally(() => {
      dispatch(setCurrenciesLoading(false));
    });
};

export const getOfficialRatesRequest = () => (dispatch, getState) => {
  if (getState().cheque.officialRatesLoading) return;

  dispatch(setOfficialRatesLoading(true));

  api
    .get("/official-rate")
    .then(({ data }) => {
      dispatch(setOfficialRates(data));
    })
    .catch((err) => {
      console.log(err);
      dispatch(setOfficialRates([]));
    })
    .finally(() => {
      dispatch(setOfficialRatesLoading(false));
    });
};

export const getBankListRequest = () => (dispatch, getState) => {
  if (getState().cheque.banksLoading) return;

  dispatch(setBanksLoading(true));

  api
    .get("/bank")
    .then(({ data }) => {
      dispatch(setBanks(data));
    })
    .catch((err) => {
      console.log(err);
      dispatch(setBanks([]));
    })
    .finally(() => {
      dispatch(setBanksLoading(false));
    });
};

export const getBranchListRequest = (bankCode) => (dispatch, getState) => {
  if (getState().cheque.branchesLoading) return;

  //dispatch(setBranchesLoading(true));

  api
    .get("/branch", {
      params: { bankCode },
    })
    .then(({ data }) => {
      dispatch(setBranches(data));
    })
    .catch((err) => {
      console.log(err);
      dispatch(setBranches([]));
    })
    .finally(() => {
      //dispatch(setBranchesLoading(false));
    });
};

const chequeQuerySlice = createSlice({
  name: "chequeQuery",
  initialState,
  reducers: {
    setCustomerList: (state, action) => {
      state.customerList = action.payload;
    },
    setDrawerList: (state, action) => {
      state.drawerList = action.payload;
    },
    setIntermediateEndorserList: (state, action) => {
      state.intermediateEndorserList = action.payload;
    },
    setLastEndorserList: (state, action) => {
      state.lastEndorserList = action.payload;
    },
    setChequeInfos: (state, action) => {
      state.chequeInfos = action.payload;
    },
    setChequeNos: (state, action) => {
      state.chequeNos = action.payload;
    },
    setTableLoading: (state, action) => {
      state.tableLoading = action.payload;
    },
    setEditCheque: (state, action) => {
      state.editCheque = action.payload;
    },
    setUpdatedCheques: (state, action) => {
      state.updatedCheques = action.payload;
    },
    setUpdatedCheque: (state, action) => {
      state.updatedCheque = action.payload;
    },
    setDeletedCheques: (state, action) => {
      state.deletedCheques = action.payload;
    },
    setChequeId: (state, action) => {
      state.chequeId = action.payload;
    },
    setCurrencies: (state, action) => {
      state.currencies = action.payload;
    },
    setCurrenciesLoading: (state, action) => {
      state.currenciesLoading = action.payload;
    },
    setOfficialRates: (state, action) => {
      state.officialRates = action.payload;
    },
    setOfficialRatesLoading: (state, action) => {
      state.officialRatesLoading = action.payload;
    },
    setBanks: (state, action) => {
      state.banks = action.payload;
    },
    setBanksLoading: (state, action) => {
      state.banksLoading = action.payload;
    },
    setBranches: (state, action) => {
      state.branches = action.payload;
    },
  },
});

export const {
  setCustomerList,
  setDrawerList,
  setIntermediateEndorserList,
  setLastEndorserList,
  setChequeInfos,
  setChequeNos,
  setTableLoading,
  setEditCheque,
  setUpdatedCheque,
  setUpdatedCheques,
  setDeletedCheques,
  setChequeId,
  setCurrencies,
  setCurrenciesLoading,
  setOfficialRates,
  setOfficialRatesLoading,
  setBanks,
  setBanksLoading,
  setBranches,
} = chequeQuerySlice.actions;

export default chequeQuerySlice.reducer;
