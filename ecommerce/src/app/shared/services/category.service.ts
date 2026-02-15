import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';

import { Observable, map } from 'rxjs';

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
    return this.http.get<any>(`${environment.baseURL}product/categories`, { params: payload }).pipe(
      map(res => {
        const categories = (res.data || res.categories || []) as ICategory[];
        return {
          data: categories,
          total: categories.length,
        } as ICategoryModel;
      }),
    );
  }

  getCategoryBySlug(slug: string): Observable<ICategory> {
    return this.getCategories().pipe(
      map(res => {
        const allCategories = res.data || [];
        // Search top-level and subcategories
        for (const cat of allCategories) {
          if (cat.slug === slug) return cat;
          if (cat.subcategories?.length) {
            const sub = cat.subcategories.find(s => s.slug === slug);
            if (sub) return sub;
          }
        }
        return allCategories[0];
      }),
    );
  }
}
