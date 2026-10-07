// Converte o texto digitado ("25,50" ou "25.50") em número.
// Retorna null se estiver vazio, não for número, for <= 0 ou tiver mais de 2 casas decimais.
export function parseAmount(text: string): number | null {
  const normalized = text.trim().replace(',', '.');

  if (!/^\d+(\.\d{1,2})?$/.test(normalized)) return null;

  const value = Number(normalized);
  return value > 0 ? value : null;
}
