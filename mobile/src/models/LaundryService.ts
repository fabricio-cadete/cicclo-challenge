export type ServiceType = 'Wash' | 'Dry';

export type LaundryService = {
  id: string;
  name: string;
  type: ServiceType;
  price: number;
};
