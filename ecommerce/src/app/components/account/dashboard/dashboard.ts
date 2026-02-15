import { AsyncPipe } from '@angular/common';
import { Component, inject } from '@angular/core';

import { Observable, catchError, map, of, shareReplay } from 'rxjs';

import { ICompanyDetailRes } from '../../../shared/interface/company-detail.interface';
import { CompanyDetailService } from '../../../shared/services/company-detail.service';

@Component({
  selector: 'app-dashboard',
  imports: [AsyncPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private companyDetailService = inject(CompanyDetailService);

  errorMessage = '';

  company$: Observable<ICompanyDetailRes | null> = this.companyDetailService
    .getCompanyDetails()
    .pipe(
      map(response => {
        const detail = response?.companyDetailRes ?? null;
        if (!detail?.companyDetail) {
          this.errorMessage = 'No company details available.';
          return null;
        }
        this.errorMessage = '';
        return detail;
      }),
      catchError(error => {
        this.errorMessage =
          error?.error?.message || 'Unable to load company details right now.';
        return of(null);
      }),
      shareReplay({ bufferSize: 1, refCount: true }),
    );

  getServiceIcon(name?: string): string {
    const value = name?.toLowerCase() ?? '';

    if (value.includes('cargo')) return 'ri-truck-line';
    if (value.includes('air') || value.includes('ticket')) return 'ri-plane-line';
    if (value.includes('market')) return 'ri-store-2-line';
    if (value.includes('delivery')) return 'ri-send-plane-2-line';

    return 'ri-service-line';
  }

  getSocialIcon(name?: string): string {
    const value = name?.toLowerCase() ?? '';

    if (value.includes('facebook')) return 'ri-facebook-fill';
    if (value.includes('instagram')) return 'ri-instagram-line';
    if (value.includes('tiktok')) return 'ri-tiktok-fill';
    if (value.includes('twitter') || value.includes('x')) return 'ri-twitter-x-line';
    if (value.includes('linkedin')) return 'ri-linkedin-fill';
    if (value.includes('youtube')) return 'ri-youtube-fill';

    return 'ri-global-line';
  }

  getPhoneLink(phone?: string): string {
    if (!phone) return '';

    const sanitized = phone.replace(/[^+\d]/g, '');
    return `tel:${sanitized || phone}`;
  }
}
