import { ChangeDetectionStrategy, Component, OnInit, inject, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Api, SoftwareLicenseRequest, SoftwareLicense } from '../../core/api';

@Component({
  selector: 'app-software-licenses',
  imports: [ReactiveFormsModule],
  templateUrl: './software-licenses.html',
  styleUrl: './software-licenses.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class SoftwareLicenses implements OnInit {
  private readonly api = inject(Api);
  private readonly fb = inject(FormBuilder);

  protected readonly licenses = signal<SoftwareLicense[]>([]);
  protected readonly errorMessage = signal('');
  protected readonly feedback = signal('');
  protected readonly isSubmitting = signal(false);

  protected readonly licenseForm = this.fb.nonNullable.group({
    name: ['', Validators.required],
    licenseKey: ['', Validators.required],
    expirationDate: ['', Validators.required],
  });

  ngOnInit(): void {
    this.loadLicenses();
  }

  protected createLicense(): void {
    this.feedback.set('');
    this.errorMessage.set('');

    if (this.licenseForm.invalid) {
      this.licenseForm.markAllAsTouched();
      return;
    }

    const raw = this.licenseForm.getRawValue();
    const payload: SoftwareLicenseRequest = {
      name: raw.name,
      licenseKey: raw.licenseKey,
      expirationDate: new Date(raw.expirationDate).toISOString(),
    };

    this.isSubmitting.set(true);

    this.api.createLicense(payload).subscribe({
      next: () => {
        this.feedback.set('Software license created successfully.');
        this.licenseForm.reset({
          name: '',
          licenseKey: '',
          expirationDate: '',
        });
        this.loadLicenses();
      },
      error: (error: unknown) => this.errorMessage.set(this.getErrorMessage(error)),
      complete: () => this.isSubmitting.set(false),
    });
  }

  private loadLicenses(): void {
    this.api.getLicenses().subscribe({
      next: (licenses) => this.licenses.set(licenses),
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
