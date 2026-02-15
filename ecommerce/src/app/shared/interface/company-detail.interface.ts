export interface ICompanyDetailResponse {
  companyDetailRes: ICompanyDetailRes;
  status: number;
  code: number;
  message: string;
}

export interface ICompanyDetailRes {
  companyDetail: ICompanyDetail;
  companyServices: ICompanyService[];
  companySocialInfos: ICompanySocialInfo[];
}

export interface ICompanyDetail {
  id: number;
  companyName: string;
  companyDescription: string;
  companyPhoneNumber: string;
  companyEmail: string;
  operationsDate: string;
  companyCode: string;
}

export interface ICompanyService {
  id: number;
  name: string;
  companyInfoId: number;
}

export interface ICompanySocialInfo {
  id: number;
  socialMediaName: string;
  socialMediaLink: string;
  socialMediaDesc: string;
  companyInfoId: number;
}
