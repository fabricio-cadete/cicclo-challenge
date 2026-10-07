export type ServiceExecution = {
  id: string;
  serviceId: string;
  price: number;
  status: 'Requested' | 'Completed' | 'Failed';
  createdAt: string;
  walletBalance: number;
};
