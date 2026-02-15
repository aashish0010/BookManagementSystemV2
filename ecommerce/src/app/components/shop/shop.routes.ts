import { Routes } from '@angular/router';

import { AuthGuard } from '../../core/guard/auth.guard';
import { CheckoutGuard } from '../../core/guard/checkout.guard';
import { BrandResolver } from '../../shared/resolver/brand.resolver';
import { CategoryResolver } from '../../shared/resolver/category.resolver';
import { ProductResolver } from '../../shared/resolver/product.resolver';

export const shop: Routes = [
  {
    path: 'cart',
    loadComponent: () => import('./cart/cart').then(m => m.Cart),
  },
  {
    path: 'collections',
    loadComponent: () => import('./collection/collection').then(m => m.Collection),
  },
  {
    path: 'product/:slug',
    loadComponent: () => import('./product/product').then(m => m.Product),
    resolve: {
      data: ProductResolver,
    },
  },
  {
    path: 'wishlist',
    loadComponent: () => import('./wishlist/wishlist').then(m => m.Wishlist),
    canActivate: [AuthGuard],
  },
  {
    path: 'compare',
    loadComponent: () => import('./compare/compare').then(m => m.Compare),
    canActivate: [AuthGuard],
  },
  {
    path: 'checkout',
    loadComponent: () => import('./checkout/checkout').then(m => m.Checkout),
    canActivate: [CheckoutGuard],
  },
  {
    path: 'order/tracking',
    loadComponent: () => import('./order-tracking/order-tracking').then(m => m.OrderTracking),
  },
  {
    path: 'order/details',
    loadComponent: () => import('./order-details/order-details').then(m => m.OrderDetails),
  },
  {
    path: 'brand/:slug',
    loadComponent: () => import('./brand/brand').then(m => m.Brand),
    resolve: {
      data: BrandResolver,
    },
  },
  {
    path: 'category/:slug',
    loadComponent: () => import('./category/category').then(m => m.Category),
    resolve: {
      data: CategoryResolver,
    },
  },
];
