import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

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
    return this.http.get<IProductModel>(`${environment.URL}/product.json`, { params: payload });
  }

  getProductBySlug(slug: string): Observable<IProduct> {
    return this.http.get<IProduct>(`${environment.URL}/product/slug/${slug}`);
  }

  getProductBySearchList(payload?: Params): Observable<IProductModel> {
    return this.http.get<IProductModel>(`${environment.URL}/product.json`, { params: payload });
  }
}
