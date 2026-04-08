import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface WarrantyCardRequest {
  provider: string;
  startDate: string;
  endDate: string;
}

export interface AssetRequest {
  name: string;
  serialNumber: string;
  statusId: number;
  employeeId: number;
  warrantyCard: WarrantyCardRequest;
}

export interface Asset extends AssetRequest {
  Id?: number;
  id?: number;
  employeeName?: string;
  statusName?: string;
}

export interface EmployeeRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface Employee extends EmployeeRequest {
  id?: number;
}

export interface SoftwareLicenseRequest {
  name: string;
  licenseKey: string;
  expirationDate: string;
}

export interface SoftwareLicense extends SoftwareLicenseRequest {
  id?: number;
}

export interface StatusRequest {
  name: string;
}

export interface StatusItem extends StatusRequest {
  id?: number;
}

@Injectable({
  providedIn: 'root',
})
export class Api {
  private readonly baseUrl = 'http://localhost:8080/api';

  constructor(private http: HttpClient) {}

  getAssets(): Observable<Asset[]> {
    return this.http.get<Asset[]>(`${this.baseUrl}/Assets`);
  }

  createAssets(data: AssetRequest): Observable<Asset> {
    return this.http.post<Asset>(`${this.baseUrl}/Assets`, data);
  }

  getEmployees(): Observable<Employee[]> {
    return this.http.get<Employee[]>(`${this.baseUrl}/Employees`);
  }

  createEmployee(data: EmployeeRequest): Observable<Employee> {
    return this.http.post<Employee>(`${this.baseUrl}/Employees`, data);
  }

  getLicenses(): Observable<SoftwareLicense[]> {
    return this.http.get<SoftwareLicense[]>(`${this.baseUrl}/SoftwareLicenses`);
  }

  createLicense(data: SoftwareLicenseRequest): Observable<SoftwareLicense> {
    return this.http.post<SoftwareLicense>(`${this.baseUrl}/SoftwareLicenses`, data);
  }

  getStatus(): Observable<StatusItem[]> {
    return this.http.get<StatusItem[]>(`${this.baseUrl}/Status`);
  }

  createStatus(data: StatusRequest): Observable<StatusItem> {
    return this.http.post<StatusItem>(`${this.baseUrl}/Status`, data);
  }
}
