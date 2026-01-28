
import { Injectable } from '@angular/core';
import { GoogleGenAI, GenerateContentResponse, Tool, Type } from '@google/genai';

export interface Message {
  role: 'user' | 'model';
  text: string;
}

@Injectable()
export class GeminiService {
  private genAI?: GoogleGenAI;
  private readonly tools: Tool[];
  private readonly apiKey: string;

  constructor() {
    this.apiKey = this.getApiKey();
    if (!this.apiKey) {
      console.error('GEMINI_API_KEY environment variable not set. AI Agent ficará desativado até a chave ser configurada.');
    } else {
      this.genAI = new GoogleGenAI({ apiKey: this.apiKey });
    }
    
    this.tools = [
      {
        functionDeclarations: [
          {
            name: 'addTransaction',
            description: 'Adiciona uma nova transação de receita (income) ou despesa (expense).',
            parameters: {
              type: Type.OBJECT,
              properties: {
                description: { type: Type.STRING, description: "A descrição da transação. Ex: 'Salário', 'Aluguel'" },
                amount: { type: Type.NUMBER, description: "O valor numérico da transação." },
                type: { type: Type.STRING, description: "O tipo da transação, deve ser 'income' para receita ou 'expense' para despesa." },
                category: { type: Type.STRING, description: "A categoria da transação. Inferir a partir da descrição se não for especificada. Exemplos: 'Moradia', 'Salário', 'Lazer', 'Alimentação'." },
                date: { type: Type.STRING, description: "A data da transação no formato AAAA-MM-DD. Se não for especificada pelo usuário, não incluir este campo."}
              },
              required: ["description", "amount", "type", "category"]
            }
          }
        ]
      }
    ];
  }

  private getApiKey(): string {
    const globalApiKey = (globalThis as { GEMINI_API_KEY?: string }).GEMINI_API_KEY;
    const importMetaApiKey = (import.meta as any).env?.GEMINI_API_KEY as string | undefined;
    const localStorageApiKey = typeof localStorage !== 'undefined' ? localStorage.getItem('GEMINI_API_KEY') ?? undefined : undefined;
    return globalApiKey ?? importMetaApiKey ?? localStorageApiKey ?? '';
  }

  hasApiKey(): boolean {
    return !!this.apiKey;
  }

  async generateFunctionCallOrText(prompt: string): Promise<GenerateContentResponse> {
    if (!this.genAI) {
      throw new Error('GEMINI_API_KEY não configurada. Defina e recarregue a página.');
    }
    const response = await this.genAI.models.generateContent({
      model: 'gemini-2.5-flash',
      contents: prompt,
      config: { tools: this.tools }
    });
    return response;
  }

  async generateTextStream(prompt: string): Promise<AsyncGenerator<string>> {
    if (!this.genAI) {
      throw new Error('GEMINI_API_KEY não configurada. Defina e recarregue a página.');
    }
    const result = await this.genAI.models.generateContentStream({
        model: "gemini-2.5-flash",
        contents: prompt,
    });
    
    const stream = (async function* () {
        for await (const chunk of result) {
            yield chunk.text;
        }
    })();

    return stream;
  }
}
