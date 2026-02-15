import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

import { Action, Selector, State, StateContext, Store } from '@ngxs/store';
import { tap } from 'rxjs';

import { IProduct, IProductModel } from '../../interface/product.interface';
import { ProductService } from '../../services/product.service';
import { ThemeOptionService } from '../../services/theme-option.service';
import {
  GetCategoryProductsAction,
  GetMenuProductsAction,
  GetMoreProductAction,
  GetProductByIdsAction,
  GetProductBySearchAction,
  GetProductBySearchListAction,
  GetProductBySlugAction,
  GetProductsAction,
  GetRelatedProductsAction,
  GetStoreProductsAction,
} from '../action/product.action';

export class ProductStateModel {
  product = {
    data: [] as IProduct[],
    total: 0,
  };
  selectedProduct: IProduct | null;
  categoryProducts: IProduct[] | [];
  relatedProducts: IProduct[] | [];
  storeProducts: IProduct[] | [];
  dealProducts: IProduct[] | [];
  menuProducts: IProduct[] | [];
  productBySearch: IProduct[] | [];
  productBySearchList: IProduct[] | [];
  productByIds: IProduct[] | [];
  moreProduct: IProduct[] | [];
}

@State<ProductStateModel>({
  name: 'product',
  defaults: {
    product: {
      data: [],
      total: 0,
    },
    selectedProduct: null,
    categoryProducts: [],
    relatedProducts: [],
    storeProducts: [],
    dealProducts: [],
    menuProducts: [],
    productBySearch: [],
    productBySearchList: [],
    productByIds: [],
    moreProduct: [],
  },
})
@Injectable()
export class ProductState {
  private store = inject(Store);
  private router = inject(Router);
  private productService = inject(ProductService);
  private themeOptionService = inject(ThemeOptionService);

  @Selector()
  static product(state: ProductStateModel) {
    return state.product;
  }

  @Selector()
  static productByIds(state: ProductStateModel) {
    return state.productByIds;
  }

  @Selector()
  static productBySearch(state: ProductStateModel) {
    return state.productBySearch;
  }

  @Selector()
  static productBySearchList(state: ProductStateModel) {
    return state.productBySearchList;
  }

  @Selector()
  static selectedProduct(state: ProductStateModel) {
    return state.selectedProduct;
  }

  @Selector()
  static relatedProducts(state: ProductStateModel) {
    return state.relatedProducts;
  }

  @Selector()
  static categoryProducts(state: ProductStateModel) {
    return state.categoryProducts;
  }

  @Selector()
  static storeProducts(state: ProductStateModel) {
    return state.storeProducts;
  }

  @Selector()
  static menuProducts(state: ProductStateModel) {
    return state.menuProducts;
  }

  @Selector()
  static moreProduct(state: ProductStateModel) {
    return state.moreProduct;
  }

  @Action(GetProductsAction)
  getProducts(ctx: StateContext<ProductStateModel>, action: GetProductsAction) {
    this.productService.skeletonLoader = true;
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          let products = result.data || [];

          if (action?.payload) {
            if (action?.payload?.['category']) {
              products = products.filter(
                product =>
                  product?.categories?.length &&
                  product.categories.some(category =>
                    action?.payload?.['category']?.split(',')?.includes(category.slug),
                  ),
              );
              products = products.length ? products : result.data;
            }

            if (action?.payload?.['sortBy']) {
              if (action?.payload?.['sortBy'] === 'asc') {
                products = products.sort((a, b) => a.id - b.id);
              } else if (action?.payload?.['sortBy'] === 'desc') {
                products = products.sort((a, b) => b.id - a.id);
              } else if (action?.payload?.['sortBy'] === 'a-z') {
                products = products.sort((a, b) => a.name.localeCompare(b.name));
              } else if (action?.payload?.['sortBy'] === 'z-a') {
                products = products.sort((a, b) => b.name.localeCompare(a.name));
              } else if (action?.payload?.['sortBy'] === 'low-high') {
                products = products.sort((a, b) => (a.sale_price || a.price) - (b.sale_price || b.price));
              } else if (action?.payload?.['sortBy'] === 'high-low') {
                products = products.sort((a, b) => (b.sale_price || b.price) - (a.sale_price || a.price));
              }
            } else {
              products = products.sort((a, b) => a.id - b.id);
            }

            if (action?.payload?.['search']) {
              products = products.filter(product =>
                product.name.toLowerCase().includes(action?.payload?.['search'].toLowerCase()),
              );
            }
          }

          ctx.patchState({
            product: {
              data: products,
              total: products.length ? products.length : result.data?.length || 0,
            },
          });
        },
        complete: () => {
          this.productService.skeletonLoader = false;
        },
        error: err => {
          this.productService.skeletonLoader = false;
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetProductByIdsAction)
  getProductByIds(ctx: StateContext<ProductStateModel>, action: GetProductByIdsAction) {
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          const state = ctx.getState();
          ctx.patchState({
            ...state,
            productByIds: result.data,
          });
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetProductBySlugAction)
  getProductBySlug(ctx: StateContext<ProductStateModel>, { slug }: GetProductBySlugAction) {
    this.themeOptionService.preloader = true;
    return this.productService.getProductBySlug(slug).pipe(
      tap({
        next: (result: IProduct) => {
          if (result) {
            result.related_products = result.related_products?.length ? result.related_products : [];
            result.cross_sell_products = result.cross_sell_products?.length ? result.cross_sell_products : [];

            const ids = [...(result.related_products || []), ...(result.cross_sell_products || [])];
            const categoryIds = result?.categories?.map(category => category.id) || [];

            if (ids.length || categoryIds.length) {
              this.store.dispatch(
                new GetRelatedProductsAction({
                  ids: ids?.join(','),
                  category_ids: categoryIds?.join(','),
                  status: 1,
                }),
              );
            }

            const state = ctx.getState();
            ctx.patchState({
              ...state,
              selectedProduct: result,
            });
          }
        },
        complete: () => {
          this.themeOptionService.preloader = false;
        },
        error: err => {
          this.themeOptionService.preloader = false;
          void this.router.navigate(['/404']);
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetRelatedProductsAction)
  getRelatedProducts(ctx: StateContext<ProductStateModel>, action: GetProductsAction) {
    this.themeOptionService.preloader = true;
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          const state = ctx.getState();
          ctx.patchState({
            ...state,
            relatedProducts: result.data,
          });
        },
        complete: () => {
          this.themeOptionService.preloader = false;
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetCategoryProductsAction)
  getCategoryProducts(ctx: StateContext<ProductStateModel>, action: GetProductsAction) {
    this.productService.skeletonCategoryProductLoader = true;
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: result => {
          const state = ctx.getState();

          result.data.map(product => {
            product['categories_ids'] = product?.categories?.map(category => category.id) || [];
          });

          let products = result.data.filter(product =>
            product?.categories_ids?.includes(action.payload!['category_id']),
          );
          products.splice(action.payload!['paginate']);

          ctx.patchState({
            ...state,
            product: {
              data: [...state.product.data, ...result.data],
              total: state.product.data.length + result.data.length,
            },
            categoryProducts: products,
          });
          this.productService.skeletonCategoryProductLoader = false;
        },
        complete: () => {
          this.productService.skeletonCategoryProductLoader = false;
          this.themeOptionService.preloader = false;
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetStoreProductsAction)
  getStoreProducts(ctx: StateContext<ProductStateModel>, action: GetProductsAction) {
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          const state = ctx.getState();
          ctx.patchState({
            ...state,
            storeProducts: result.data,
          });
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetMenuProductsAction)
  getMenuProducts(ctx: StateContext<ProductStateModel>, action: GetMenuProductsAction) {
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          const state = ctx.getState();
          ctx.patchState({
            ...state,
            menuProducts: result.data,
          });
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetProductBySearchAction)
  getProductBySearch(ctx: StateContext<ProductStateModel>, action: GetProductBySearchAction) {
    this.productService.searchSkeleton = true;
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: result => {
          let products;
          if (action?.payload?.['search']) {
            products = result.data.filter(product =>
              product.name.toLowerCase().includes(action?.payload?.['search'].toLowerCase()),
            );
          } else {
            products = result.data;
          }

          ctx.patchState({
            productBySearch: products.splice(0, 4),
          });
        },
        complete: () => {
          this.productService.searchSkeleton = false;
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetProductBySearchListAction)
  getProductBySearchList(
    ctx: StateContext<ProductStateModel>,
    action: GetProductBySearchListAction,
  ) {
    this.productService.searchSkeleton = true;
    return this.productService.getProductBySearchList(action.payload).pipe(
      tap({
        next: result => {
          ctx.patchState({
            productBySearchList: result.data,
          });
        },
        complete: () => {
          this.productService.searchSkeleton = false;
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }

  @Action(GetMoreProductAction)
  getMoreProduct(ctx: StateContext<ProductStateModel>, action: GetMoreProductAction) {
    return this.productService.getProducts(action.payload).pipe(
      tap({
        next: (result: IProductModel) => {
          const state = ctx.getState();

          result.data.map(product => {
            product['categories_ids'] = product?.categories?.map(category => category.id) || [];
          });

          let filteredProducts = result.data.filter(product =>
            action.payload!['category_id']?.some((category_id: number) =>
              product.categories_ids.includes(category_id),
            ),
          );

          const page = action.payload!['page'];
          const itemsPerPage = action.payload!['paginate'];

          const startIndex = (page - 1) * itemsPerPage;
          const endIndex = startIndex + itemsPerPage;

          let paginatedProducts = filteredProducts.length
            ? filteredProducts
            : result.data.slice(startIndex, endIndex);
          if (action.value) {
            ctx.patchState({
              moreProduct: [...state.moreProduct, ...paginatedProducts],
            });
          } else {
            ctx.patchState({
              moreProduct: [...paginatedProducts],
            });
          }
        },
        error: err => {
          throw new Error(err?.error?.message);
        },
      }),
    );
  }
}
