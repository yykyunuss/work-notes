import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { store } from "./index";

type GlobalState = {
  workProcessId?: number;
  userCode?: string;
  correspondentBanks: {
    [currency: string]: string[];
  };
};

const initialState: GlobalState = {
  workProcessId: undefined,
  correspondentBanks: {},
  userCode: "",
};

export const globalSlice = createSlice({
  name: "globalData",
  initialState,
  reducers: {
    setWorkProcessId: (state: GlobalState, action: PayloadAction<number>) => {
      state.workProcessId = action.payload;
    },
    setUserCode: (state: GlobalState, action: PayloadAction<string>) => {
      state.userCode = action.payload;
    },
  },
});

export type RootState = ReturnType<typeof store.getState>;
export default globalSlice.reducer;
export const globalDataStore = (state: RootState) => state.globalReducer;
