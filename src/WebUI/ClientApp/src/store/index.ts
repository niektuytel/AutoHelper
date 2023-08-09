import * as Products from "./products/ProductsReducer";
import * as TagFilters from "./tag_filter/TagFilterReducer";
import * as TagSituations from "./tag_filtering/TagFilteringReducer";
import * as RESTStatus from "./status/StatusReducer";
import * as Cart from "./cart/CartReducer";
import * as Checkout from "./checkout/CheckoutReducer";
import ProductsState from "./products/ProductsState";
import TagFilterState from "./tag_filter/TagFilterState";
import TagSituationState from "./tag_filtering/TagFilteringState";
import StatusState from "./status/StatusState"
import CartState from "./cart/CartState";
import CheckoutState from "./checkout/CheckoutState";

// The top-level state object
export interface ApplicationState {
    checkout: CheckoutState
    cart: CartState;
    status: StatusState;
    products: ProductsState | undefined;
    tag_filter: TagFilterState | undefined;
    tag_filtering: TagSituationState;
}

// Whenever an action is dispatched, Redux will update each top-level application state property using
// the reducer with the matching name. It's important that the names match exactly, and that the reducer
// acts on the corresponding ApplicationState property type.
export const reducers = {
    checkout: Checkout.reducer,
    cart: Cart.reducer,
    status: RESTStatus.reducer,
    products: Products.reducer,
    tag_filters: TagFilters.reducer,
    tag_filtering: TagSituations.reducer
};

// This type can be used as a hint on action creators so that its 'dispatch' and 'getState' params are
// correctly typed to match your store.
export interface AppThunkAction<TAction> {
    (dispatch: (action: TAction) => void, getState: () => ApplicationState): void;
}
