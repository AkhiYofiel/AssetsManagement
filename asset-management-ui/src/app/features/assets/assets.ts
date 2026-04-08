import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Api, Asset, AssetRequest, Employee, StatusItem } from '../../core/api';

@Component({
  selector: 'app-assets',
  imports: [ReactiveFormsModule],
  templateUrl: './assets.html',
  styleUrl: './assets.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Assets implements OnInit {
  private readonly api = inject(Api);
  private readonly fb = inject(FormBuilder);

  protected readonly assets = signal<Asset[]>([]);
  protected readonly employees = signal<Employee[]>([]);
  protected readonly statuses = signal<StatusItem[]>([]);
  protected readonly errorMessage = signal('');
  protected readonly feedback = signal('');
  protected readonly isSubmitting = signal(false);

  protected readonly assetForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    serialNumber: ['', Validators.required],
    statusId: [0, [Validators.required, Validators.min(1)]],
    employeeId: [0, [Validators.required, Validators.min(1)]],
    provider: ['', Validators.required],
    startDate: ['', Validators.required],
    endDate: ['', Validators.required],
  });

  ngOnInit(): void {
    this.loadAssets();
    this.loadEmployees();
    this.loadStatus();
  }

  protected createAsset(): void {
    this.feedback.set('');
    this.errorMessage.set('');

    if (this.assetForm.invalid) {
      this.assetForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);

    const formValue = this.assetForm.getRawValue();
    const payload: AssetRequest = {
      name: formValue.name,
      serialNumber: formValue.serialNumber,
      statusId: Number(formValue.statusId),
      employeeId: Number(formValue.employeeId),
      warrantyCard: {
        provider: formValue.provider,
        startDate: new Date(formValue.startDate).toISOString(),
        endDate: new Date(formValue.endDate).toISOString(),
      },
    };

    this.api.createAssets(payload).subscribe({
      next: () => {
        this.feedback.set('Asset created successfully.');
        this.assetForm.reset({
          name: '',
          serialNumber: '',
          statusId: 0,
          employeeId: 0,
          provider: '',
          startDate: '',
          endDate: '',
        });
        this.loadAssets();
      },
      error: (error: unknown) => {
        this.errorMessage.set(this.getErrorMessage(error));
      },
      complete: () => {
        this.isSubmitting.set(false);
      },
    });
  }

  private loadAssets(): void {
    this.api.getAssets().subscribe({
      next: (assets) => this.assets.set(assets),
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
    });
  }

  private loadEmployees(): void {
    this.api.getEmployees().subscribe({
      next: (employees) => this.employees.set(employees),
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
    });
  }

  private loadStatus(): void {
    this.api.getStatus().subscribe({
      next: (status) => this.statuses.set(status),
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
    });
  }

  private getErrorMessage(error: unknown): string {
    if (error && typeof error === 'object' && 'message' in error) {
      return String(error.message);
    }

    return 'Request failed. Please check API availability and try again.';
  }
}
