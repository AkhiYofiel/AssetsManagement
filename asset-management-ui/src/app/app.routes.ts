import { Routes } from '@angular/router';

export const routes: Routes = [
	{
		path: '',
		redirectTo: 'assets',
		pathMatch: 'full',
	},
	{
		path: 'assets',
		loadComponent: () =>
			import('./features/assets/assets').then((m) => m.Assets),
	},
	{
		path: 'employees',
		loadComponent: () =>
			import('./features/employees/employees').then((m) => m.Employees),
	},
	{
		path: 'software-licenses',
		loadComponent: () =>
			import('./features/software-licenses/software-licenses').then(
				(m) => m.SoftwareLicenses,
			),
	},
	{
		path: 'status',
		loadComponent: () => import('./features/status/status').then((m) => m.Status),
	},
];
