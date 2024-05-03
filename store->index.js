import { configureStore } from "@reduxjs/toolkit";
import cheque from "./chequeSlice";
import customer from "./customerSlice";
import decisionSupport from "./decisionSupportSlice";
import dsmAutoLimit from "./dsmAutoLimitSlice";
import chequeQuery from "./chequeQuerySlice";

export const store = configureStore({
  reducer: {
    cheque,
    customer,
    dsmAutoLimit,
    decisionSupport,
    chequeQuery,
  },
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
    }),
});
