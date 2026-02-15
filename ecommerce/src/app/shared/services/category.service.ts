import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable } from 'rxjs';

import { environment } from '../../../environments/environment.development';
import { ICategory, ICategoryModel } from '../interface/category.interface';
import { Params } from '../interface/core.interface';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private http = inject(HttpClient);

  public searchSkeleton: boolean = false;

  getCategories(payload?: Params): Observable<ICategoryModel> {
    return this.http.get<ICategoryModel>(`${environment.URL}/category.json`, { params: payload });
  }

  getCategoryBySlug(slug: string): Observable<ICategory> {
    return this.http.get<ICategory>(`${environment.URL}/category/slug/${slug}`);
  }
}
