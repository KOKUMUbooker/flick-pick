// import adapter from '@sveltejs/adapter-netlify';

// /** @type {import('@sveltejs/kit').Config} */
// const config = {
// 	kit: {
// 		adapter: adapter(),
// 		alias: {
// 			'@/*': './src/lib/*'
// 		}
// 	}
// };

// export default config;

// import adapter from '@sveltejs/adapter-auto';

// /** @type {import('@sveltejs/kit').Config} */
// const config = {
// 	kit: {
// 		// adapter-auto only supports some environments, see https://svelte.dev/docs/kit/adapter-auto for a list.
// 		// If your environment is not supported, or you settled on a specific environment, switch out the adapter.
// 		// See https://svelte.dev/docs/kit/adapters for more information about adapters.
// 		adapter: adapter(),
// 		outDir: '../wwwroot',
// 		output: { bundleStrategy: 'split' },
// 		alias: {
// 			'@/*': './src/lib/*'
// 		}
// 	}
// };

// export default config;

import adapter from '@sveltejs/adapter-static';

/** @type {import('@sveltejs/kit').Config} */
const config = {
	kit: {
		adapter: adapter({
			pages: '../wwwroot',
			assets: '../wwwroot',
			fallback: 'index.html',
			precompress: false,
			strict: false
		}),
		alias: {
			'@/*': './src/lib/*'
		}
	}
};

export default config;
