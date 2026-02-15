import { isPlatformBrowser } from '@angular/common';
import { Component, PLATFORM_ID, inject, input } from '@angular/core';

import { Store } from '@ngxs/store';
import { forkJoin, of } from 'rxjs';

import { environment } from '../../../../environments/environment';
import { Categories } from '../../../shared/components/widgets/categories/categories';
import { IBooks } from '../../../shared/interface/theme.interface';
import { ThemeOptionService } from '../../../shared/services/theme-option.service';
import { GetBrandsAction } from '../../../shared/store/action/brand.action';
import { GetCategoriesAction } from '../../../shared/store/action/category.action';
import { GetProductByIdsAction } from '../../../shared/store/action/product.action';
import { ThemeBrand } from '../widgets/theme-brand/theme-brand';
import { ThemeFourColumnProduct } from '../widgets/theme-four-column-product/theme-four-column-product';
import { ThemeHomeSlider } from '../widgets/theme-home-slider/theme-home-slider';
import { ThemeProductTabSection } from '../widgets/theme-product-tab-section/theme-product-tab-section';
import { ThemeTitle } from '../widgets/theme-title/theme-title';

@Component({
  selector: 'app-books',
  imports: [
    ThemeHomeSlider,
    Categories,
    ThemeTitle,
    ThemeProductTabSection,
    ThemeFourColumnProduct,
    ThemeBrand,
  ],
  templateUrl: './books.html',
  styleUrl: './books.scss',
})
export class Books {
  private store = inject(Store);
  private themeOptionService = inject(ThemeOptionService);

  readonly data = input<IBooks>();
  readonly slug = input<string>();
  private platformId: boolean;
  public StorageURL = environment.storageURL;

  constructor() {
    const platformId = inject<Object>(PLATFORM_ID);

    this.platformId = isPlatformBrowser(platformId);
  }

  ngOnInit() {
    const data = this.data();
    if (data?.slug == this.slug()) {
      let categoryIds = [
        ...new Set([
          ...(data?.content?.categories_1?.category_ids || []),
          ...(data?.content?.category_product?.category_ids || []),
          ...(data?.content?.categories_2?.category_ids || []),
          ...(data?.content?.products_list?.category_ids || []),
          ...(data?.content?.everyday_casual?.category_ids || []),
        ]),
      ];

      // Get Products
      let getProducts$;
      if (
        data?.content?.products_ids?.length &&
        (data?.content?.products_list?.status || data?.content?.slider_products?.status)
      ) {
        getProducts$ = this.store.dispatch(
          new GetProductByIdsAction({
            status: 1,
            approve: 1,
            ids: data?.content?.products_ids?.join(','),
            paginate: data?.content?.products_ids?.length,
          }),
        );
      } else {
        getProducts$ = of(null);
      }

      // Get Category
      let getCategory$;
      if (
        categoryIds.length &&
        (data?.content?.category_product?.status ||
          data?.content?.categories_1?.category_ids ||
          data?.content?.categories_2?.category_ids)
      ) {
        getCategory$ = this.store.dispatch(
          new GetCategoriesAction({
            status: 1,
            ids: categoryIds?.join(','),
          }),
        );
      } else {
        getCategory$ = of(null);
      }

      // Get Brand
      let getBrands$;
      if (data?.content?.brand?.brand_ids?.length && data?.content?.brand?.status) {
        getBrands$ = this.store.dispatch(
          new GetBrandsAction({
            status: 1,
            ids: data?.content?.brand?.brand_ids?.join(','),
          }),
        );
      } else {
        getBrands$ = of(null);
      }

      // Skeleton Loader
      if (this.platformId) {
        document.body.classList.add('skeleton-body');
        forkJoin([getProducts$, getCategory$, getBrands$]).subscribe({
          complete: () => {
            document.body.classList.remove('skeleton-body');
            this.themeOptionService.preloader = false;
          },
        });
      }
    }

    // header light
    if (this.platformId) {
      document.body.classList.add('header-style-light');
    }
  }

  ngOnDestroy() {
    if (this.platformId) {
      document.body.classList.remove('header-style-light');
    }
  }
}
