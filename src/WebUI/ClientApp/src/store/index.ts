import { combineReducers, configureStore } from "@reduxjs/toolkit";
import { createReduxHistoryContext } from "redux-first-history";
import { createBrowserHistory } from "history";

const {
    routerReducer,
    routerMiddleware,
    createReduxHistory
} = createReduxHistoryContext({
    history: createBrowserHistory() as any,
    batch: (fn:any) => Promise.resolve().then(fn)
});

const rootReducer = combineReducers({ router: routerReducer });
export const store = configureStore({
    reducer: rootReducer,
    middleware: (getDefaultMiddleware) =>
        getDefaultMiddleware().concat(routerMiddleware)
});

export const history = createReduxHistory(store) as any;