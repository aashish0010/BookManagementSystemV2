import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable, map } from 'rxjs';

import { environment } from '../../../environments/environment.development';
import { Params } from '../interface/core.interface';
import { IProduct, IProductModel } from '../interface/product.interface';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private http = inject(HttpClient);

  public skeletonLoader: boolean = false;
  public skeletonCategoryProductLoader: boolean = false;
  public productFilter: boolean = false;
  public searchSkeleton: boolean = false;

  getProducts(payload?: Params): Observable<IProductModel> {
    return this.http.get<any>(`${environment.baseURL}product`, { params: payload }).pipe(
      map(res => ({
        data: (res.data || res.products || []) as IProduct[],
        total: res.total || 0,
        current_page: res.page || 1,
        per_page: res.pageSize || 20,
        last_page: res.totalPages || 1,
      } as IProductModel)),
    );
  }

  getProductBySlug(slug: string): Observable<IProduct> {
    return this.http.get<any>(`${environment.baseURL}product/${slug}`).pipe(
      map(res => (res.product || res) as IProduct),
    );
  }

  getProductBySearchList(payload?: Params): Observable<IProductModel> {
    return this.http.get<any>(`${environment.baseURL}product`, { params: payload }).pipe(
      map(res => ({
        data: (res.data || res.products || []) as IProduct[],
        total: res.total || 0,
        current_page: res.page || 1,
        per_page: res.pageSize || 20,
        last_page: res.totalPages || 1,
      } as IProductModel)),
    );
  }
}
